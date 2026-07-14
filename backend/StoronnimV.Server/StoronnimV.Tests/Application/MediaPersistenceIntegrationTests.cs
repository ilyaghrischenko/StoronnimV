using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Options;
using StoronnimV.Application.Services.Utils;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Infrastructure;
using StoronnimV.Infrastructure.Repositories.AzureBlobStorage;
using StoronnimV.Infrastructure.Repositories.Database;
using Xunit.Sdk;

namespace StoronnimV.Tests.Application;

public sealed class MediaPersistenceIntegrationTests
{
    [Fact]
    public async Task CreateReplaceDelete_RealPostgresAndAzurite_KeepDbAndBlobStateExplainable()
    {
        // Arrange
        (string dbConnection, string blobConnection) = RequireIntegrationEnvironment();
        string prefix = $"data04-it-{Guid.NewGuid():N}";
        string platformUrl = $"https://example.test/{prefix}";
        BlobRepository blobRepository = new();
        MediaStorageService mediaStorageService = new(
            new MediaFileValidator(Options.Create(CreatePolicy())),
            blobRepository);

        try
        {
            // Act: create.
            MusicPlatform platform = new() { BgImageUrl = string.Empty, PlatformUrl = platformUrl };
            string createdUrl = await mediaStorageService.CreateAsync(
                CreateFile("photo.jpg", "image/jpeg", [0xFF, 0xD8, 0xFF, 0xE0]),
                MediaKind.Photo,
                prefix,
                async photoUrl =>
                {
                    platform.BgImageUrl = photoUrl;
                    await using StoronnimVContext context = CreateContext(dbConnection);
                    context.MusicPlatforms.Add(platform);
                    await context.SaveChangesAsync();
                },
                CancellationToken.None);

            // Assert: create and content type.
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Assert.Equal(createdUrl, (await context.MusicPlatforms.SingleAsync(x => x.PlatformUrl == platformUrl)).BgImageUrl);
            }
            Assert.Equal("image/jpeg", await GetContentTypeAsync(blobConnection, createdUrl));

            // Act/Assert: invalid replacement is rejected before Blob/DB mutation.
            await Assert.ThrowsAsync<MediaValidationException>(() => mediaStorageService.ReplaceAsync(
                CreateFile("photo.jpg", "image/jpeg", [0x00, 0x01, 0x02, 0x03]),
                MediaKind.Photo,
                prefix,
                createdUrl,
                _ => throw new InvalidOperationException("Database callback must not run."),
                CancellationToken.None));
            Assert.Equal(1, await CountBlobsAsync(blobConnection, prefix));

            // Act/Assert: forced DB failure rolls the new Blob back.
            await Assert.ThrowsAsync<DbUpdateException>(() => mediaStorageService.CreateAsync(
                CreateFile("photo.jpg", "image/jpeg", [0xFF, 0xD8, 0xFF, 0xE0]),
                MediaKind.Photo,
                $"{prefix}-db-failure",
                async photoUrl =>
                {
                    await using StoronnimVContext context = CreateContext(dbConnection);
                    context.MusicPlatforms.Add(new MusicPlatform { BgImageUrl = photoUrl, PlatformUrl = null! });
                    await context.SaveChangesAsync();
                },
                CancellationToken.None));
            Assert.Equal(0, await CountBlobsAsync(blobConnection, $"{prefix}-db-failure"));

            // Act: replace.
            string replacementUrl = await mediaStorageService.ReplaceAsync(
                CreateFile("photo.png", "image/png", [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]),
                MediaKind.Photo,
                prefix,
                createdUrl,
                async photoUrl =>
                {
                    await using StoronnimVContext context = CreateContext(dbConnection);
                    MusicPlatform stored = await context.MusicPlatforms.SingleAsync(x => x.PlatformUrl == platformUrl);
                    stored.BgImageUrl = photoUrl;
                    await context.SaveChangesAsync();
                },
                CancellationToken.None);

            // Assert: DB switched before old Blob cleanup and only replacement remains.
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Assert.Equal(replacementUrl, (await context.MusicPlatforms.SingleAsync(x => x.PlatformUrl == platformUrl)).BgImageUrl);
            }
            Assert.Equal("image/png", await GetContentTypeAsync(blobConnection, replacementUrl));
            Assert.Equal(1, await CountBlobsAsync(blobConnection, prefix));

            // Act: delete.
            await mediaStorageService.DeleteAsync(
                MediaKind.Photo,
                replacementUrl,
                async () =>
                {
                    await using StoronnimVContext context = CreateContext(dbConnection);
                    MusicPlatform stored = await context.MusicPlatforms.SingleAsync(x => x.PlatformUrl == platformUrl);
                    context.MusicPlatforms.Remove(stored);
                    await context.SaveChangesAsync();
                },
                CancellationToken.None);

            // Assert: both states are removed.
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Assert.False(await context.MusicPlatforms.AnyAsync(x => x.PlatformUrl == platformUrl));
            }
            Assert.Equal(0, await CountBlobsAsync(blobConnection, prefix));
        }
        finally
        {
            await CleanupAsync(dbConnection, blobConnection, prefix, platformUrl);
        }
    }

    [Fact]
    public async Task ReplacePromotion_RealPostgresFailure_PreservesOldRow()
    {
        // Arrange
        (string dbConnection, _) = RequireIntegrationEnvironment();
        string marker = $"data04-promotion-{Guid.NewGuid():N}";
        long oldId;
        await using (StoronnimVContext context = CreateContext(dbConnection))
        {
            Video oldPromotion = new()
            {
                Title = marker,
                Url = $"https://example.test/{marker}.mp4",
                BlobName = $"{marker}.mp4",
                Type = VideoType.Promotion
            };
            context.Videos.Add(oldPromotion);
            await context.SaveChangesAsync();
            oldId = oldPromotion.Id;
        }

        try
        {
            // Act.
            await using StoronnimVContext context = CreateContext(dbConnection);
            Video current = await context.Videos.SingleAsync(x => x.Id == oldId);
            VideoRepository repository = new(context);
            Video invalidReplacement = new()
            {
                Title = null!,
                Url = $"https://example.test/{marker}-replacement.mp4",
                BlobName = $"{marker}-replacement.mp4",
                Type = VideoType.Promotion
            };
            await Assert.ThrowsAsync<DbUpdateException>(() =>
                repository.ReplacePromotionAsync(current, invalidReplacement, CancellationToken.None));

            // Assert.
            await using StoronnimVContext verificationContext = CreateContext(dbConnection);
            Assert.True(await verificationContext.Videos.AnyAsync(x => x.Id == oldId && x.Title == marker));
            Assert.False(await verificationContext.Videos.AnyAsync(x => x.BlobName == invalidReplacement.BlobName));
        }
        finally
        {
            await using StoronnimVContext context = CreateContext(dbConnection);
            await context.Videos.Where(x => x.Title == marker || x.BlobName.StartsWith(marker)).ExecuteDeleteAsync();
        }
    }

    private static (string DbConnection, string BlobConnection) RequireIntegrationEnvironment()
    {
        if (Environment.GetEnvironmentVariable("DATA04_INTEGRATION") != "1")
        {
            throw SkipException.ForSkip("Set DATA04_INTEGRATION=1 with disposable DB_CLOUD and BLOB_STORAGE targets.");
        }

        string dbConnection = Environment.GetEnvironmentVariable("DB_CLOUD")
            ?? throw new InvalidOperationException("DB_CLOUD is required for DATA-04 integration tests.");
        string blobConnection = Environment.GetEnvironmentVariable("BLOB_STORAGE")
            ?? throw new InvalidOperationException("BLOB_STORAGE is required for DATA-04 integration tests.");
        return (dbConnection, blobConnection);
    }

    private static StoronnimVContext CreateContext(string connectionString)
    {
        DbContextOptions<StoronnimVContext> options = new DbContextOptionsBuilder<StoronnimVContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new StoronnimVContext(options);
    }

    private static MediaUploadOptions CreatePolicy() => new()
    {
        MaxPhotoBytes = MediaUploadOptions.MaxPhotoBytesLimit,
        MaxVideoBytes = MediaUploadOptions.MaxVideoBytesLimit,
        AllowedPhotoContentTypes = ["image/jpeg", "image/png", "image/webp"],
        AllowedVideoContentTypes = ["video/mp4"]
    };

    private static IFormFile CreateFile(string fileName, string contentType, byte[] bytes)
    {
        MemoryStream stream = new(bytes);
        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static async Task<string> GetContentTypeAsync(string connectionString, string url)
    {
        string blobName = Uri.UnescapeDataString(new Uri(url).Segments[^1]);
        BlobClient blob = new BlobServiceClient(connectionString)
            .GetBlobContainerClient("storonnimv-photo")
            .GetBlobClient(blobName);
        return (await blob.GetPropertiesAsync()).Value.ContentType;
    }

    private static async Task<int> CountBlobsAsync(string connectionString, string prefix)
    {
        BlobContainerClient container = new BlobServiceClient(connectionString)
            .GetBlobContainerClient("storonnimv-photo");
        int count = 0;
        await foreach (var _ in container.GetBlobsAsync(prefix: prefix))
        {
            count++;
        }
        return count;
    }

    private static async Task CleanupAsync(
        string dbConnection,
        string blobConnection,
        string prefix,
        string platformUrl)
    {
        await using (StoronnimVContext context = CreateContext(dbConnection))
        {
            await context.MusicPlatforms.Where(x => x.PlatformUrl == platformUrl).ExecuteDeleteAsync();
        }

        BlobContainerClient container = new BlobServiceClient(blobConnection)
            .GetBlobContainerClient("storonnimv-photo");
        await foreach (var blob in container.GetBlobsAsync(prefix: prefix))
        {
            await container.DeleteBlobIfExistsAsync(blob.Name);
        }
    }
}
