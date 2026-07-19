using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using StoronnimV.Application.DTO.Responses.Shared;
using StoronnimV.Application.DTO.Responses.Video;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Infrastructure;
using Xunit.Sdk;

namespace StoronnimV.Tests.Api;

public sealed class VideoCrudIntegrationTests(AuthApiFactory factory)
    : IClassFixture<AuthApiFactory>
{
    private const string BlobConnection = "UseDevelopmentStorage=true";

    [Fact]
    public async Task VideoVertical_RealApiPostgresAndAzurite_PreservesCategoriesCrudAndPromotion()
    {
        string dbConnection = RequireIntegrationEnvironment();
        string marker = $"feat08-{Guid.NewGuid():N}";
        List<string> blobNames = [];

        try
        {
            BlobContainerClient videoContainer = new BlobServiceClient(BlobConnection)
                .GetBlobContainerClient("storonnimv-video");
            await videoContainer.CreateIfNotExistsAsync();
            await videoContainer.SetAccessPolicyAsync(PublicAccessType.Blob);

            using HttpClient client = factory.CreateApiClient();
            string token = factory.CreateToken("Basic");

            Video performanceOne = await CreateVideoAsync(
                client, token, dbConnection, $"{marker}-performance-1", VideoType.Performance);
            Video performanceTwo = await CreateVideoAsync(
                client, token, dbConnection, $"{marker}-performance-2", VideoType.Performance);
            Video backstage = await CreateVideoAsync(
                client, token, dbConnection, $"{marker}-backstage", VideoType.Backstage);
            Video repetition = await CreateVideoAsync(
                client, token, dbConnection, $"{marker}-repetition", VideoType.Repetition);
            blobNames.AddRange([performanceOne.BlobName, performanceTwo.BlobName, backstage.BlobName, repetition.BlobName]);

            foreach (VideoType category in new[]
                     {
                         VideoType.Performance,
                         VideoType.Backstage,
                         VideoType.Repetition
                     })
            {
                PaginationResponse<VideoPageResponse> page = await GetPageAsync(client, category, 1, 100);
                Assert.Contains(page.Items, item => item.Title.StartsWith(marker));
                Assert.All(page.Items.Where(item => item.Title.StartsWith(marker)),
                    item => Assert.Equal(category.ToString(), item.Type));
            }

            PaginationResponse<VideoPageResponse> firstPerformancePage =
                await GetPageAsync(client, VideoType.Performance, 1, 1);
            PaginationResponse<VideoPageResponse> secondPerformancePage =
                await GetPageAsync(client, VideoType.Performance, 2, 1);
            Assert.NotEqual(
                Assert.Single(firstPerformancePage.Items).Id,
                Assert.Single(secondPerformancePage.Items).Id);

            PaginationResponse<VideoPageResponse> outOfRange =
                await GetPageAsync(client, VideoType.Performance, 999, 1);
            Assert.Empty(outOfRange.Items);
            Assert.Equal(999, outOfRange.CurrentPage);
            Assert.True(outOfRange.TotalItems >= 2);
            Assert.True(outOfRange.TotalPages >= 2);

            Assert.Equal(
                HttpStatusCode.BadRequest,
                (await client.GetAsync("/api/videos/page/Performance/0?pageSize=1")).StatusCode);
            Assert.Equal(
                HttpStatusCode.BadRequest,
                (await client.GetAsync("/api/videos/page/Performance/1?pageSize=0")).StatusCode);
            Assert.Equal(
                HttpStatusCode.BadRequest,
                (await client.GetAsync("/api/videos/page/Unknown/1?pageSize=1")).StatusCode);

            VideoPageResponse performanceDetail = await GetVideoAsync(client, performanceOne.Id);
            Assert.Equal(performanceOne.Url, performanceDetail.Url);
            Assert.Equal("Performance", performanceDetail.Type);
            Assert.Equal(HttpStatusCode.OK, (await new HttpClient().GetAsync(performanceDetail.Url)).StatusCode);

            using (HttpRequestMessage editRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/videos",
                       token,
                       JsonContent.Create(new
                       {
                           id = performanceOne.Id,
                           title = $"{marker}-edited",
                           type = "Backstage"
                       })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(editRequest)).StatusCode);
            }

            VideoPageResponse edited = await GetVideoAsync(client, performanceOne.Id);
            Assert.Equal($"{marker}-edited", edited.Title);
            Assert.Equal("Backstage", edited.Type);
            Assert.Contains(
                (await GetPageAsync(client, VideoType.Backstage, 1, 100)).Items,
                item => item.Id == performanceOne.Id);

            Video oldPromotion = await CreateVideoAsync(
                client, token, dbConnection, $"{marker}-promotion-old", VideoType.Promotion);
            blobNames.Add(oldPromotion.BlobName);
            Assert.Equal(oldPromotion.Id, (await GetPromotionAsync(client)).Id);

            using (MultipartFormDataContent invalidReplacement = new()
                   {
                       { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "video/mp4"), "url", "invalid.mp4" },
                       { new StringContent($"{marker}-promotion-invalid"), "title" },
                       { new StringContent("Promotion"), "type" }
                   })
            using (HttpRequestMessage invalidRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/videos", token, invalidReplacement))
            {
                Assert.Equal(HttpStatusCode.UnsupportedMediaType, (await client.SendAsync(invalidRequest)).StatusCode);
            }

            Assert.Equal(oldPromotion.Id, (await GetPromotionAsync(client)).Id);
            Assert.True(await BlobExistsAsync(oldPromotion.BlobName));

            Video replacementPromotion = await CreateVideoAsync(
                client, token, dbConnection, $"{marker}-promotion-new", VideoType.Promotion);
            blobNames.Add(replacementPromotion.BlobName);
            VideoPageResponse promotionReadback = await GetPromotionAsync(client);
            Assert.Equal(replacementPromotion.Id, promotionReadback.Id);
            Assert.Equal($"{marker}-promotion-new", promotionReadback.Title);
            Assert.False(await BlobExistsAsync(oldPromotion.BlobName));
            Assert.True(await BlobExistsAsync(replacementPromotion.BlobName));

            foreach (long id in new[] { performanceOne.Id, performanceTwo.Id, backstage.Id, repetition.Id })
            {
                using HttpRequestMessage deleteRequest = Authenticated(
                    HttpMethod.Delete, $"/api/admin/videos/{id}", token);
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteRequest)).StatusCode);
                Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/videos/{id}")).StatusCode);
            }

            foreach (string blobName in new[]
                     {
                         performanceOne.BlobName,
                         performanceTwo.BlobName,
                         backstage.BlobName,
                         repetition.BlobName
                     })
            {
                Assert.False(await BlobExistsAsync(blobName));
            }

            PaginationResponse<VideoPageResponse> emptyRepetition =
                await GetPageAsync(client, VideoType.Repetition, 1, 100);
            Assert.Empty(emptyRepetition.Items);
            Assert.Equal(0, emptyRepetition.TotalItems);
            Assert.Equal(0, emptyRepetition.TotalPages);
        }
        finally
        {
            await CleanupAsync(dbConnection, marker, blobNames);
        }
    }

    private static string RequireIntegrationEnvironment()
    {
        if (Environment.GetEnvironmentVariable("FEAT08_INTEGRATION") != "1")
        {
            throw SkipException.ForSkip(
                "Set FEAT08_INTEGRATION=1 with disposable DB_CLOUD and local Azurite targets.");
        }

        return Environment.GetEnvironmentVariable("DB_CLOUD")
               ?? throw new InvalidOperationException("DB_CLOUD is required for FEAT-08 integration tests.");
    }

    private static async Task<Video> CreateVideoAsync(
        HttpClient client,
        string token,
        string dbConnection,
        string title,
        VideoType type)
    {
        using MultipartFormDataContent content = new()
        {
            { CreateFileContent([0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70], "video/mp4"), "url", $"{title}.mp4" },
            { new StringContent(title), "title" },
            { new StringContent(type.ToString()), "type" }
        };
        using HttpRequestMessage request = Authenticated(HttpMethod.Post, "/api/admin/videos", token, content);

        Assert.Equal(HttpStatusCode.Created, (await client.SendAsync(request)).StatusCode);

        await using StoronnimVContext context = CreateContext(dbConnection);
        return await context.Videos.AsNoTracking().SingleAsync(video => video.Title == title);
    }

    private static async Task<PaginationResponse<VideoPageResponse>> GetPageAsync(
        HttpClient client,
        VideoType type,
        int page,
        int pageSize)
    {
        PaginationResponse<VideoPageResponse>? response = await client.GetFromJsonAsync<
            PaginationResponse<VideoPageResponse>>($"/api/videos/page/{type}/{page}?pageSize={pageSize}");
        return Assert.IsType<PaginationResponse<VideoPageResponse>>(response);
    }

    private static async Task<VideoPageResponse> GetVideoAsync(HttpClient client, long id)
    {
        HttpResponseMessage response = await client.GetAsync($"/api/videos/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsType<VideoPageResponse>(await response.Content.ReadFromJsonAsync<VideoPageResponse>());
    }

    private static async Task<VideoPageResponse> GetPromotionAsync(HttpClient client)
    {
        HttpResponseMessage response = await client.GetAsync("/api/home/video");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsType<VideoPageResponse>(await response.Content.ReadFromJsonAsync<VideoPageResponse>());
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

    private static ByteArrayContent CreateFileContent(byte[] bytes, string contentType)
    {
        ByteArrayContent content = new(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return content;
    }

    private static StoronnimVContext CreateContext(string connectionString)
    {
        DbContextOptions<StoronnimVContext> options = new DbContextOptionsBuilder<StoronnimVContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new StoronnimVContext(options);
    }

    private static async Task<bool> BlobExistsAsync(string blobName)
    {
        return await new BlobServiceClient(BlobConnection)
            .GetBlobContainerClient("storonnimv-video")
            .GetBlobClient(blobName)
            .ExistsAsync();
    }

    private static async Task CleanupAsync(
        string dbConnection,
        string marker,
        IEnumerable<string> blobNames)
    {
        await using (StoronnimVContext context = CreateContext(dbConnection))
        {
            await context.Videos.Where(video => video.Title.StartsWith(marker)).ExecuteDeleteAsync();
        }

        BlobContainerClient container = new BlobServiceClient(BlobConnection)
            .GetBlobContainerClient("storonnimv-video");
        foreach (string blobName in blobNames.Distinct())
        {
            await container.DeleteBlobIfExistsAsync(blobName);
        }
    }
}
