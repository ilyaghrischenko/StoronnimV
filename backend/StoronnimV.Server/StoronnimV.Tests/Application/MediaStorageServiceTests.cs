using Microsoft.AspNetCore.Http;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Models;
using StoronnimV.Application.Services.Utils;
using StoronnimV.Domain.Contracts.AzureBlobStorage;

namespace StoronnimV.Tests.Application;

public sealed class MediaStorageServiceTests
{
    [Fact]
    public async Task CreateAsync_DatabasePersistFails_DeletesUploadedBlob()
    {
        // Arrange
        RecordingBlobRepository blobRepository = new();
        MediaStorageService service = CreateService(blobRepository);
        IFormFile file = CreatePhoto();

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(
            file,
            MediaKind.Photo,
            "news",
            _ =>
            {
                blobRepository.Operations.Add("database");
                throw new InvalidOperationException("database failed");
            },
            CancellationToken.None));

        // Assert
        Assert.Empty(blobRepository.StoredFiles);
        Assert.Equal(["validate", "upload", "database", "delete"], blobRepository.Operations);
    }

    [Fact]
    public async Task CreateAsync_DatabaseAndRollbackFail_ReportsRequiredCleanup()
    {
        // Arrange
        RecordingBlobRepository blobRepository = new() { FailDelete = true };
        MediaStorageService service = CreateService(blobRepository);
        IFormFile file = CreatePhoto();

        // Act
        MediaConsistencyException exception = await Assert.ThrowsAsync<MediaConsistencyException>(() => service.CreateAsync(
            file,
            MediaKind.Photo,
            "news",
            _ =>
            {
                blobRepository.Operations.Add("database");
                throw new InvalidOperationException("database failed");
            },
            CancellationToken.None));

        // Assert
        Assert.Equal("storonnimv-photo", exception.ContainerName);
        Assert.StartsWith("news-", exception.BlobName, StringComparison.Ordinal);
        Assert.Single(blobRepository.StoredFiles);
    }

    [Fact]
    public async Task ReplaceAsync_UploadFails_DoesNotChangeDatabaseOrOldBlob()
    {
        // Arrange
        const string oldBlobName = "news-old.jpg";
        RecordingBlobRepository blobRepository = new() { FailUpload = true };
        blobRepository.StoredFiles.Add(("storonnimv-photo", oldBlobName));
        MediaStorageService service = CreateService(blobRepository);
        Boolean databaseCalled = false;

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReplaceAsync(
            CreatePhoto(),
            MediaKind.Photo,
            "news",
            $"https://storage.test/storonnimv-photo/{oldBlobName}",
            _ =>
            {
                databaseCalled = true;
                return Task.CompletedTask;
            },
            CancellationToken.None));

        // Assert
        Assert.False(databaseCalled);
        Assert.Contains(("storonnimv-photo", oldBlobName), blobRepository.StoredFiles);
        Assert.DoesNotContain("delete", blobRepository.Operations);
    }

    [Fact]
    public async Task ReplaceAsync_MalformedOldUrl_DoesNotUploadOrChangeDatabase()
    {
        // Arrange
        RecordingBlobRepository blobRepository = new();
        MediaStorageService service = CreateService(blobRepository);

        // Act
        MediaConsistencyException exception = await Assert.ThrowsAsync<MediaConsistencyException>(() =>
            service.ReplaceAsync(
                CreatePhoto(),
                MediaKind.Photo,
                "news",
                "default",
                _ =>
                {
                    blobRepository.Operations.Add("database");
                    return Task.CompletedTask;
                },
                CancellationToken.None));

        // Assert
        Assert.Equal("storonnimv-photo", exception.ContainerName);
        Assert.Equal("unknown", exception.BlobName);
        Assert.Empty(blobRepository.Operations);
        Assert.Empty(blobRepository.StoredFiles);
    }

    [Fact]
    public async Task ReplaceAsync_DatabaseFails_DeletesNewBlobAndPreservesOldBlob()
    {
        // Arrange
        const string oldBlobName = "news-old.jpg";
        RecordingBlobRepository blobRepository = new();
        blobRepository.StoredFiles.Add(("storonnimv-photo", oldBlobName));
        MediaStorageService service = CreateService(blobRepository);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReplaceAsync(
            CreatePhoto(),
            MediaKind.Photo,
            "news",
            $"https://storage.test/storonnimv-photo/{oldBlobName}",
            _ =>
            {
                blobRepository.Operations.Add("database");
                throw new InvalidOperationException("database failed");
            },
            CancellationToken.None));

        // Assert
        Assert.Equal([("storonnimv-photo", oldBlobName)], blobRepository.StoredFiles);
        Assert.Equal(["validate", "upload", "database", "delete"], blobRepository.Operations);
    }

    [Fact]
    public async Task ReplaceAsync_Succeeds_ChangesDatabaseBeforeDeletingOldBlob()
    {
        // Arrange
        const string oldBlobName = "news-old.jpg";
        RecordingBlobRepository blobRepository = new();
        blobRepository.StoredFiles.Add(("storonnimv-photo", oldBlobName));
        MediaStorageService service = CreateService(blobRepository);

        // Act
        string newUrl = await service.ReplaceAsync(
            CreatePhoto(),
            MediaKind.Photo,
            "news",
            $"https://storage.test/storonnimv-photo/{oldBlobName}",
            _ =>
            {
                blobRepository.Operations.Add("database");
                return Task.CompletedTask;
            },
            CancellationToken.None);

        // Assert
        Assert.DoesNotContain(("storonnimv-photo", oldBlobName), blobRepository.StoredFiles);
        Assert.Contains(blobRepository.StoredFiles, file => newUrl.EndsWith(file.BlobName, StringComparison.Ordinal));
        Assert.Equal(["validate", "upload", "database", "delete"], blobRepository.Operations);
    }

    [Fact]
    public async Task DeleteAsync_DatabaseFails_PreservesBlob()
    {
        // Arrange
        const string oldBlobName = "news-old.jpg";
        RecordingBlobRepository blobRepository = new();
        blobRepository.StoredFiles.Add(("storonnimv-photo", oldBlobName));
        MediaStorageService service = CreateService(blobRepository);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(
            MediaKind.Photo,
            $"https://storage.test/storonnimv-photo/{oldBlobName}",
            () => throw new InvalidOperationException("database failed"),
            CancellationToken.None));

        // Assert
        Assert.Contains(("storonnimv-photo", oldBlobName), blobRepository.StoredFiles);
        Assert.DoesNotContain("delete", blobRepository.Operations);
    }

    [Fact]
    public async Task DeleteAsync_MalformedOldUrl_DoesNotChangeDatabase()
    {
        // Arrange
        RecordingBlobRepository blobRepository = new();
        MediaStorageService service = CreateService(blobRepository);

        // Act
        MediaConsistencyException exception = await Assert.ThrowsAsync<MediaConsistencyException>(() =>
            service.DeleteAsync(
                MediaKind.Photo,
                "default",
                () =>
                {
                    blobRepository.Operations.Add("database");
                    return Task.CompletedTask;
                },
                CancellationToken.None));

        // Assert
        Assert.Equal("storonnimv-photo", exception.ContainerName);
        Assert.Equal("unknown", exception.BlobName);
        Assert.Empty(blobRepository.Operations);
    }

    [Fact]
    public async Task DeleteAsync_Succeeds_DeletesBlobAfterDatabaseMutation()
    {
        // Arrange
        const string oldBlobName = "news-old.jpg";
        RecordingBlobRepository blobRepository = new();
        blobRepository.StoredFiles.Add(("storonnimv-photo", oldBlobName));
        MediaStorageService service = CreateService(blobRepository);

        // Act
        await service.DeleteAsync(
            MediaKind.Photo,
            $"https://storage.test/storonnimv-photo/{oldBlobName}",
            () =>
            {
                blobRepository.Operations.Add("database");
                return Task.CompletedTask;
            },
            CancellationToken.None);

        // Assert
        Assert.Empty(blobRepository.StoredFiles);
        Assert.Equal(["database", "delete"], blobRepository.Operations);
    }

    [Fact]
    public async Task DeleteAsync_RequestCancelledAfterDatabaseMutation_CompletesBlobCleanup()
    {
        // Arrange
        const string oldBlobName = "news-old.jpg";
        using CancellationTokenSource cancellation = new();
        RecordingBlobRepository blobRepository = new();
        blobRepository.StoredFiles.Add(("storonnimv-photo", oldBlobName));
        MediaStorageService service = CreateService(blobRepository);

        // Act
        await service.DeleteAsync(
            MediaKind.Photo,
            $"https://storage.test/storonnimv-photo/{oldBlobName}",
            () =>
            {
                blobRepository.Operations.Add("database");
                cancellation.Cancel();
                return Task.CompletedTask;
            },
            cancellation.Token);

        // Assert
        Assert.Empty(blobRepository.StoredFiles);
        Assert.Equal(["database", "delete"], blobRepository.Operations);
    }

    [Fact]
    public async Task DeleteAsync_BlobCleanupFails_ReportsOrphanAfterDatabaseMutation()
    {
        // Arrange
        const string oldBlobName = "news-old.jpg";
        RecordingBlobRepository blobRepository = new() { FailDelete = true };
        blobRepository.StoredFiles.Add(("storonnimv-photo", oldBlobName));
        MediaStorageService service = CreateService(blobRepository);

        // Act
        MediaCleanupException exception = await Assert.ThrowsAsync<MediaCleanupException>(() => service.DeleteAsync(
            MediaKind.Photo,
            $"https://storage.test/storonnimv-photo/{oldBlobName}",
            () =>
            {
                blobRepository.Operations.Add("database");
                return Task.CompletedTask;
            },
            CancellationToken.None));

        // Assert
        Assert.Equal("storonnimv-photo", exception.ContainerName);
        Assert.Equal(oldBlobName, exception.BlobName);
        Assert.Equal(["database", "delete"], blobRepository.Operations);
        Assert.Contains(("storonnimv-photo", oldBlobName), blobRepository.StoredFiles);
    }

    private static MediaStorageService CreateService(RecordingBlobRepository blobRepository)
    {
        return new MediaStorageService(new RecordingMediaFileValidator(blobRepository.Operations), blobRepository);
    }

    private static IFormFile CreatePhoto()
    {
        MemoryStream stream = new([0xFF, 0xD8, 0xFF, 0xE0]);
        return new FormFile(stream, 0, stream.Length, "file", "photo.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }

    private sealed class RecordingMediaFileValidator(List<string> operations) : IMediaFileValidator
    {
        public Task ValidateAsync(IFormFile file, MediaKind mediaKind, CancellationToken ct)
        {
            operations.Add("validate");
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingBlobRepository : IBlobRepository
    {
        public HashSet<(string ContainerName, string BlobName)> StoredFiles { get; } = [];
        public List<string> Operations { get; } = [];
        public bool FailUpload { get; init; }
        public bool FailDelete { get; init; }

        public Task<string> AddFileAndGetUrlAsync(
            string containerName,
            string fileName,
            Stream fileStream,
            string contentType,
            CancellationToken ct)
        {
            Operations.Add("upload");
            if (FailUpload)
            {
                throw new InvalidOperationException("upload failed");
            }

            StoredFiles.Add((containerName, fileName));
            return Task.FromResult($"https://storage.test/{containerName}/{fileName}");
        }

        public string GetFileUrl(string containerName, string fileName, CancellationToken ct) =>
            $"https://storage.test/{containerName}/{fileName}";

        public Task DeleteFileAsync(string containerName, string fileName, CancellationToken ct)
        {
            Operations.Add("delete");
            ct.ThrowIfCancellationRequested();
            if (FailDelete)
            {
                throw new InvalidOperationException("delete failed");
            }

            StoredFiles.Remove((containerName, fileName));
            return Task.CompletedTask;
        }

    }
}
