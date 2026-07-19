using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using StoronnimV.Application.DTO.Responses;
using StoronnimV.Application.DTO.Responses.MusicPage;
using StoronnimV.Domain.Entities;
using StoronnimV.Infrastructure;
using Xunit.Sdk;

namespace StoronnimV.Tests.Api;

public sealed class MusicAndGroupSocialCrudIntegrationTests(AuthApiFactory factory)
    : IClassFixture<AuthApiFactory>
{
    [Fact]
    public async Task MusicAndGroupSocialCrud_RealApiPostgresAndAzurite_ValidatesLinksAndPersistsPhotoReadback()
    {
        (string dbConnection, string blobConnection) = RequireIntegrationEnvironment();
        string marker = $"feat07-{Guid.NewGuid():N}";
        List<string> blobUrls = [];

        try
        {
            BlobContainerClient photoContainer = new BlobServiceClient(blobConnection)
                .GetBlobContainerClient("storonnimv-photo");
            await photoContainer.CreateIfNotExistsAsync();
            await photoContainer.SetAccessPolicyAsync(PublicAccessType.Blob);

            using HttpClient client = factory.CreateApiClient();
            string token = factory.CreateToken("Basic");

            using (MultipartFormDataContent createMusicContent = new()
            {
                { new StringContent($"https://music.example/{marker}/created"), "platformUrl" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "bgImageUrl", "music-created.jpg" }
            })
            using (HttpRequestMessage createMusicRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/music", token, createMusicContent))
            {
                Assert.Equal(HttpStatusCode.Created, (await client.SendAsync(createMusicRequest)).StatusCode);
            }

            MusicPlatform musicEntity;
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                musicEntity = await context.MusicPlatforms.SingleAsync(
                    x => x.PlatformUrl == $"https://music.example/{marker}/created");
                blobUrls.Add(musicEntity.BgImageUrl);
            }

            MusicResponse createdMusic = Assert.Single(
                await GetMusicAsync(client), item => item.Id == musicEntity.Id);
            Assert.Equal($"https://music.example/{marker}/created", createdMusic.PlatformUrl);
            Assert.Equal(blobUrls[0], createdMusic.BgImageUrl);
            Assert.True(await BlobExistsAsync(blobConnection, blobUrls[0]));

            using (MultipartFormDataContent invalidMusicContent = new()
            {
                { new StringContent($"javascript:alert('{marker}')"), "platformUrl" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "bgImageUrl", "music-invalid.jpg" }
            })
            using (HttpRequestMessage invalidMusicRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/music", token, invalidMusicContent))
            {
                Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(invalidMusicRequest)).StatusCode);
            }

            using (HttpRequestMessage editMusicRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/music-platforms",
                       token,
                       JsonContent.Create(new
                       {
                           id = musicEntity.Id,
                           platformUrl = $"http://music.example/{marker}/edited"
                       })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(editMusicRequest)).StatusCode);
            }

            using (HttpRequestMessage invalidMusicEditRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/music-platforms",
                       token,
                       JsonContent.Create(new
                       {
                           id = musicEntity.Id,
                           platformUrl = $"ftp://example.test/{marker}/file"
                       })))
            {
                Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(invalidMusicEditRequest)).StatusCode);
            }
            Assert.Equal(
                $"http://music.example/{marker}/edited",
                Assert.Single(await GetMusicAsync(client), item => item.Id == musicEntity.Id).PlatformUrl);

            using (MultipartFormDataContent musicPhotoContent = new()
            {
                { new StringContent(musicEntity.Id.ToString()), "id" },
                { CreateFileContent([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image/png"), "photo", "music-edited.png" }
            })
            using (HttpRequestMessage musicPhotoRequest = Authenticated(
                       HttpMethod.Patch, "/api/admin/music-platforms/photo", token, musicPhotoContent))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(musicPhotoRequest)).StatusCode);
            }

            string editedMusicPhoto = Assert.Single(
                await GetMusicAsync(client), item => item.Id == musicEntity.Id).BgImageUrl;
            blobUrls.Add(editedMusicPhoto);
            Assert.NotEqual(blobUrls[0], editedMusicPhoto);
            Assert.False(await BlobExistsAsync(blobConnection, blobUrls[0]));
            Assert.True(await BlobExistsAsync(blobConnection, editedMusicPhoto));

            using (MultipartFormDataContent createSocialContent = new()
            {
                { new StringContent("YouTube"), "name" },
                { new StringContent($"https://social.example/{marker}/created"), "linkUrl" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "photo", "social-created.jpg" }
            })
            using (HttpRequestMessage createSocialRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/group-socials", token, createSocialContent))
            {
                Assert.Equal(HttpStatusCode.Created, (await client.SendAsync(createSocialRequest)).StatusCode);
            }

            GroupSocial socialEntity;
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                socialEntity = await context.GroupSocials.SingleAsync(
                    x => x.LinkUrl == $"https://social.example/{marker}/created");
                blobUrls.Add(socialEntity.PhotoUrl);
            }

            GroupSocialResponse createdSocial = Assert.Single(
                await GetGroupSocialsAsync(client), item => item.Id == socialEntity.Id);
            Assert.Equal("YouTube", createdSocial.Name);
            Assert.Equal($"https://social.example/{marker}/created", createdSocial.LinkUrl);
            Assert.Equal(blobUrls[2], createdSocial.PhotoUrl);
            Assert.True(await BlobExistsAsync(blobConnection, blobUrls[2]));

            using (MultipartFormDataContent invalidSocialContent = new()
            {
                { new StringContent("Other"), "name" },
                { new StringContent($"example.test/{marker}/no-scheme"), "linkUrl" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "photo", "social-invalid.jpg" }
            })
            using (HttpRequestMessage invalidSocialRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/group-socials", token, invalidSocialContent))
            {
                Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(invalidSocialRequest)).StatusCode);
            }

            using (HttpRequestMessage editSocialRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/group-socials",
                       token,
                       JsonContent.Create(new
                       {
                           id = socialEntity.Id,
                           linkUrl = $"https://social.example/{marker}/edited"
                       })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(editSocialRequest)).StatusCode);
            }

            using (HttpRequestMessage invalidSocialEditRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/group-socials",
                       token,
                       JsonContent.Create(new
                       {
                           id = socialEntity.Id,
                           linkUrl = $"javascript:alert('{marker}')"
                       })))
            {
                Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(invalidSocialEditRequest)).StatusCode);
            }
            Assert.Equal(
                $"https://social.example/{marker}/edited",
                Assert.Single(await GetGroupSocialsAsync(client), item => item.Id == socialEntity.Id).LinkUrl);

            using (MultipartFormDataContent socialPhotoContent = new()
            {
                { new StringContent(socialEntity.Id.ToString()), "id" },
                { CreateFileContent([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image/png"), "photo", "social-edited.png" }
            })
            using (HttpRequestMessage socialPhotoRequest = Authenticated(
                       HttpMethod.Patch, "/api/admin/group-socials/photo", token, socialPhotoContent))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(socialPhotoRequest)).StatusCode);
            }

            string editedSocialPhoto = Assert.Single(
                await GetGroupSocialsAsync(client), item => item.Id == socialEntity.Id).PhotoUrl;
            blobUrls.Add(editedSocialPhoto);
            Assert.NotEqual(blobUrls[2], editedSocialPhoto);
            Assert.False(await BlobExistsAsync(blobConnection, blobUrls[2]));
            Assert.True(await BlobExistsAsync(blobConnection, editedSocialPhoto));

            using (HttpRequestMessage deleteSocialRequest = Authenticated(
                       HttpMethod.Delete, $"/api/admin/group-socials/{socialEntity.Id}", token))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteSocialRequest)).StatusCode);
            }
            Assert.DoesNotContain(await GetGroupSocialsAsync(client), item => item.Id == socialEntity.Id);
            Assert.Equal(
                HttpStatusCode.NotFound,
                (await client.GetAsync($"/api/group-socials/{socialEntity.Id}")).StatusCode);
            Assert.False(await BlobExistsAsync(blobConnection, editedSocialPhoto));

            using (HttpRequestMessage deleteMusicRequest = Authenticated(
                       HttpMethod.Delete, $"/api/admin/music/{musicEntity.Id}", token))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteMusicRequest)).StatusCode);
            }
            Assert.DoesNotContain(await GetMusicAsync(client), item => item.Id == musicEntity.Id);
            Assert.Equal(
                HttpStatusCode.NotFound,
                (await client.GetAsync($"/api/music/{musicEntity.Id}")).StatusCode);
            Assert.False(await BlobExistsAsync(blobConnection, editedMusicPhoto));
        }
        finally
        {
            await CleanupAsync(dbConnection, blobConnection, marker, blobUrls);
        }
    }

    private static (string DbConnection, string BlobConnection) RequireIntegrationEnvironment()
    {
        if (Environment.GetEnvironmentVariable("FEAT07_INTEGRATION") != "1")
        {
            throw SkipException.ForSkip(
                "Set FEAT07_INTEGRATION=1 with disposable DB_CLOUD and BLOB_STORAGE targets.");
        }

        string dbConnection = Environment.GetEnvironmentVariable("DB_CLOUD")
            ?? throw new InvalidOperationException("DB_CLOUD is required for FEAT-07 integration tests.");
        string blobConnection = Environment.GetEnvironmentVariable("BLOB_STORAGE")
            ?? throw new InvalidOperationException("BLOB_STORAGE is required for FEAT-07 integration tests.");
        return (dbConnection, blobConnection);
    }

    private static StoronnimVContext CreateContext(string connectionString)
    {
        DbContextOptions<StoronnimVContext> options = new DbContextOptionsBuilder<StoronnimVContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new StoronnimVContext(options);
    }

    private static ByteArrayContent CreateFileContent(byte[] bytes, string contentType)
    {
        ByteArrayContent content = new(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return content;
    }

    private static HttpRequestMessage Authenticated(
        HttpMethod method,
        string route,
        string token,
        HttpContent? content = null)
    {
        HttpRequestMessage request = new(method, route) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static async Task<IReadOnlyList<MusicResponse>> GetMusicAsync(HttpClient client)
    {
        HttpResponseMessage response = await client.GetAsync("/api/music");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsAssignableFrom<IReadOnlyList<MusicResponse>>(
            await response.Content.ReadFromJsonAsync<List<MusicResponse>>());
    }

    private static async Task<IReadOnlyList<GroupSocialResponse>> GetGroupSocialsAsync(HttpClient client)
    {
        HttpResponseMessage response = await client.GetAsync("/api/group-socials");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsAssignableFrom<IReadOnlyList<GroupSocialResponse>>(
            await response.Content.ReadFromJsonAsync<List<GroupSocialResponse>>());
    }

    private static async Task<bool> BlobExistsAsync(string connectionString, string url)
    {
        Uri uri = new(url);
        string containerName = uri.Segments[^2].Trim('/');
        string blobName = Uri.UnescapeDataString(uri.Segments[^1]);
        return await new BlobServiceClient(connectionString)
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName)
            .ExistsAsync();
    }

    private static async Task CleanupAsync(
        string dbConnection,
        string blobConnection,
        string marker,
        List<string> blobUrls)
    {
        await using (StoronnimVContext context = CreateContext(dbConnection))
        {
            blobUrls.AddRange(await context.MusicPlatforms
                .Where(x => x.PlatformUrl.Contains(marker))
                .Select(x => x.BgImageUrl)
                .ToListAsync());
            blobUrls.AddRange(await context.GroupSocials
                .Where(x => x.LinkUrl.Contains(marker))
                .Select(x => x.PhotoUrl)
                .ToListAsync());

            await context.MusicPlatforms
                .Where(x => x.PlatformUrl.Contains(marker))
                .ExecuteDeleteAsync();
            await context.GroupSocials
                .Where(x => x.LinkUrl.Contains(marker))
                .ExecuteDeleteAsync();
        }

        foreach (string url in blobUrls.Distinct())
        {
            Uri uri = new(url);
            string containerName = uri.Segments[^2].Trim('/');
            string blobName = Uri.UnescapeDataString(uri.Segments[^1]);
            await new BlobServiceClient(blobConnection)
                .GetBlobContainerClient(containerName)
                .DeleteBlobIfExistsAsync(blobName);
        }
    }
}
