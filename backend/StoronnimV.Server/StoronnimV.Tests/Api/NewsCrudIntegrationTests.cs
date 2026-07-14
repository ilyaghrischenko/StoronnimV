using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using StoronnimV.Application.DTO.Responses.NewsPage;
using StoronnimV.Application.DTO.Responses.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Infrastructure;
using Xunit.Sdk;

namespace StoronnimV.Tests.Api;

public sealed class NewsCrudIntegrationTests(AuthApiFactory factory)
    : IClassFixture<AuthApiFactory>
{
    [Fact]
    public async Task NewsCrud_RealApiPostgresAndAzurite_PreservesDateMediaAndReadback()
    {
        (string dbConnection, string blobConnection) = RequireIntegrationEnvironment();
        string marker = $"feat04-{Guid.NewGuid():N}";
        List<string> blobUrls = [];
        long firstVideoId;
        long secondVideoId;

        await using (StoronnimVContext context = CreateContext(dbConnection))
        {
            Video firstVideo = new()
            {
                Title = $"{marker}-video-1",
                Url = $"https://example.test/{marker}-1.mp4",
                BlobName = $"{marker}-1.mp4",
                Type = VideoType.Performance
            };
            Video secondVideo = new()
            {
                Title = $"{marker}-video-2",
                Url = $"https://example.test/{marker}-2.mp4",
                BlobName = $"{marker}-2.mp4",
                Type = VideoType.Performance
            };
            context.Videos.AddRange(firstVideo, secondVideo);
            await context.SaveChangesAsync();
            firstVideoId = firstVideo.Id;
            secondVideoId = secondVideo.Id;
        }

        try
        {
            using HttpClient client = factory.CreateApiClient();
            string token = factory.CreateToken("Basic");

            using (MultipartFormDataContent createContent = new()
            {
                { new StringContent($"{marker}-created"), "title" },
                { new StringContent("Created description"), "description" },
                { new StringContent("Main"), "priority" },
                { new StringContent("2026-07-14"), "date" },
                { new StringContent(firstVideoId.ToString()), "videoId" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "photo", "created.jpg" }
            })
            using (HttpRequestMessage createRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/news", token, createContent))
            {
                HttpResponseMessage createResponse = await client.SendAsync(createRequest);
                Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            }

            long newsId;
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                News created = await context.NewsItems.SingleAsync(x => x.Title == $"{marker}-created");
                newsId = created.Id;
                blobUrls.Add(Assert.IsType<string>(created.Photo));
            }

            PaginationResponse<NewsShortResponse>? page = await client.GetFromJsonAsync<
                PaginationResponse<NewsShortResponse>>("/api/news/page/1?pageSize=100");
            NewsShortResponse createdListItem = Assert.Single(
                Assert.IsType<PaginationResponse<NewsShortResponse>>(page).Items,
                item => item.Id == newsId);
            Assert.Equal("14.07.2026", createdListItem.Date);

            PaginationResponse<NewsShortResponse>? outOfRangePage = await client.GetFromJsonAsync<
                PaginationResponse<NewsShortResponse>>("/api/news/page/999?pageSize=100");
            PaginationResponse<NewsShortResponse> outOfRange =
                Assert.IsType<PaginationResponse<NewsShortResponse>>(outOfRangePage);
            Assert.Empty(outOfRange.Items);
            Assert.Equal(999, outOfRange.CurrentPage);
            Assert.True(outOfRange.TotalItems >= 1);
            Assert.True(outOfRange.TotalPages >= 1);

            NewsResponse createdDetail = await GetNewsAsync(client, newsId);
            Assert.Equal("14.07.2026", createdDetail.Date);
            Assert.Equal($"https://example.test/{marker}-1.mp4", createdDetail.Video);
            Assert.Equal(blobUrls[0], createdDetail.Photo);

            using (HttpRequestMessage editRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/news",
                       token,
                       JsonContent.Create(new
                       {
                           id = newsId,
                           title = $"{marker}-edited",
                           description = "Edited description",
                           priority = "Secondary",
                           date = "2026-08-15"
                       })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(editRequest)).StatusCode);
            }

            NewsResponse editedDetail = await GetNewsAsync(client, newsId);
            Assert.Equal($"{marker}-edited", editedDetail.Title);
            Assert.Equal("15.08.2026", editedDetail.Date);

            using (MultipartFormDataContent photoContent = new()
            {
                { new StringContent(newsId.ToString()), "id" },
                { CreateFileContent([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image/png"), "photo", "replacement.png" }
            })
            using (HttpRequestMessage photoRequest = Authenticated(
                       HttpMethod.Patch, "/api/admin/news/photo", token, photoContent))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(photoRequest)).StatusCode);
            }

            NewsResponse photoDetail = await GetNewsAsync(client, newsId);
            string replacementPhoto = Assert.IsType<string>(photoDetail.Photo);
            blobUrls.Add(replacementPhoto);
            Assert.NotEqual(blobUrls[0], replacementPhoto);
            Assert.False(await BlobExistsAsync(blobConnection, blobUrls[0]));
            Assert.True(await BlobExistsAsync(blobConnection, replacementPhoto));

            using (HttpRequestMessage videoRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/news/video",
                       token,
                       JsonContent.Create(new { id = newsId, videoId = secondVideoId })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(videoRequest)).StatusCode);
            }
            Assert.Equal($"https://example.test/{marker}-2.mp4", (await GetNewsAsync(client, newsId)).Video);

            using (HttpRequestMessage deleteVideoRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/news/delete-video",
                       token,
                       JsonContent.Create(newsId)))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteVideoRequest)).StatusCode);
            }
            Assert.Null((await GetNewsAsync(client, newsId)).Video);

            using (HttpRequestMessage deletePhotoRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/news/delete-photo",
                       token,
                       JsonContent.Create(newsId)))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deletePhotoRequest)).StatusCode);
            }
            Assert.Null((await GetNewsAsync(client, newsId)).Photo);
            Assert.False(await BlobExistsAsync(blobConnection, replacementPhoto));

            using (HttpRequestMessage deleteRequest = Authenticated(
                       HttpMethod.Delete, $"/api/admin/news/{newsId}", token))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteRequest)).StatusCode);
            }
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/news/{newsId}")).StatusCode);
        }
        finally
        {
            await CleanupAsync(dbConnection, blobConnection, marker, blobUrls);
        }
    }

    private static (string DbConnection, string BlobConnection) RequireIntegrationEnvironment()
    {
        if (Environment.GetEnvironmentVariable("FEAT04_INTEGRATION") != "1")
        {
            throw SkipException.ForSkip(
                "Set FEAT04_INTEGRATION=1 with disposable DB_CLOUD and BLOB_STORAGE targets.");
        }

        string dbConnection = Environment.GetEnvironmentVariable("DB_CLOUD")
            ?? throw new InvalidOperationException("DB_CLOUD is required for FEAT-04 integration tests.");
        string blobConnection = Environment.GetEnvironmentVariable("BLOB_STORAGE")
            ?? throw new InvalidOperationException("BLOB_STORAGE is required for FEAT-04 integration tests.");
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

    private static async Task<NewsResponse> GetNewsAsync(HttpClient client, long id)
    {
        HttpResponseMessage response = await client.GetAsync($"/api/news/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsType<NewsResponse>(await response.Content.ReadFromJsonAsync<NewsResponse>());
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
        IEnumerable<string> blobUrls)
    {
        await using (StoronnimVContext context = CreateContext(dbConnection))
        {
            await context.NewsItems.Where(x => x.Title.StartsWith(marker)).ExecuteDeleteAsync();
            await context.Videos.Where(x => x.Title.StartsWith(marker)).ExecuteDeleteAsync();
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
