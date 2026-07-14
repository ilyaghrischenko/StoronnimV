using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using StoronnimV.Application.DTO.Responses.SchedulePage;
using StoronnimV.Application.DTO.Responses.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Infrastructure;
using Xunit.Sdk;

namespace StoronnimV.Tests.Api;

public sealed class ScheduleCrudIntegrationTests(AuthApiFactory factory)
    : IClassFixture<AuthApiFactory>
{
    [Fact]
    public async Task ScheduleCrud_RealApiPostgresAndAzurite_PreservesDateStatusPhotoAndReadback()
    {
        (string dbConnection, string blobConnection) = RequireIntegrationEnvironment();
        string marker = $"feat05-{Guid.NewGuid():N}";
        List<string> blobUrls = [];
        long scheduleId = 0;

        try
        {
            BlobContainerClient photoContainer = new BlobServiceClient(blobConnection)
                .GetBlobContainerClient("storonnimv-photo");
            await photoContainer.CreateIfNotExistsAsync();
            await photoContainer.SetAccessPolicyAsync(PublicAccessType.Blob);

            using HttpClient client = factory.CreateApiClient();
            string token = factory.CreateToken("Basic");

            using (MultipartFormDataContent createContent = new()
            {
                { new StringContent($"{marker}-created"), "title" },
                { new StringContent("Created description"), "description" },
                { new StringContent("Kyiv, Ukraine"), "location" },
                { new StringContent("Active"), "status" },
                { new StringContent("2036-07-14T19:30"), "performanceDateTime" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "photo", "created.jpg" }
            })
            using (HttpRequestMessage createRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/schedules", token, createContent))
            {
                HttpResponseMessage createResponse = await client.SendAsync(createRequest);
                Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            }

            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Schedule created = await context.Schedules.SingleAsync(x => x.Title == $"{marker}-created");
                scheduleId = created.Id;
                Assert.Equal(DateTimeKind.Utc, created.PerformanceDateTime.Kind);
                blobUrls.Add(Assert.IsType<string>(created.Photo));
            }

            using (HttpClient publicClient = new())
            {
                Assert.Equal(HttpStatusCode.OK, (await publicClient.GetAsync(blobUrls[0])).StatusCode);
            }

            PaginationResponse<ScheduleShortResponse>? page = await client.GetFromJsonAsync<
                PaginationResponse<ScheduleShortResponse>>("/api/schedules/page/1?pageSize=100");
            ScheduleShortResponse createdListItem = Assert.Single(
                Assert.IsType<PaginationResponse<ScheduleShortResponse>>(page).Items,
                item => item.Id == scheduleId);
            Assert.Equal("14.07.2036 19:30", createdListItem.PerformanceDateTime);
            Assert.Equal("Active", createdListItem.Status);
            Assert.Equal("Kyiv, Ukraine", createdListItem.Location);
            Assert.Equal(blobUrls[0], createdListItem.Photo);

            PaginationResponse<ScheduleShortResponse>? outOfRangePage = await client.GetFromJsonAsync<
                PaginationResponse<ScheduleShortResponse>>("/api/schedules/page/999?pageSize=100");
            PaginationResponse<ScheduleShortResponse> outOfRange =
                Assert.IsType<PaginationResponse<ScheduleShortResponse>>(outOfRangePage);
            Assert.Empty(outOfRange.Items);
            Assert.Equal(999, outOfRange.CurrentPage);
            Assert.True(outOfRange.TotalItems >= 1);
            Assert.True(outOfRange.TotalPages >= 1);

            Assert.Equal(
                HttpStatusCode.BadRequest,
                (await client.GetAsync("/api/schedules/page/0?pageSize=100")).StatusCode);
            Assert.Equal(
                HttpStatusCode.BadRequest,
                (await client.GetAsync("/api/schedules/page/1?pageSize=0")).StatusCode);

            ScheduleResponse createdDetail = await GetScheduleAsync(client, scheduleId);
            Assert.Equal("14.07.2036 19:30", createdDetail.PerformanceDateTime);
            Assert.Equal("Active", createdDetail.Status);
            Assert.Equal("Kyiv, Ukraine", createdDetail.Location);
            Assert.Equal(blobUrls[0], createdDetail.Photo);

            using (HttpRequestMessage editRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/schedules",
                       token,
                       JsonContent.Create(new
                       {
                           id = scheduleId,
                           title = $"{marker}-edited",
                           description = "Edited description",
                           location = "Lviv, Ukraine",
                           performanceDateTime = "2036-08-15T20:45"
                       })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(editRequest)).StatusCode);
            }

            ScheduleResponse editedDetail = await GetScheduleAsync(client, scheduleId);
            Assert.Equal($"{marker}-edited", editedDetail.Title);
            Assert.Equal("15.08.2036 20:45", editedDetail.PerformanceDateTime);
            Assert.Equal("Lviv, Ukraine", editedDetail.Location);
            Assert.Equal("Active", editedDetail.Status);

            using (MultipartFormDataContent photoContent = new()
            {
                { new StringContent(scheduleId.ToString()), "id" },
                { CreateFileContent([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image/png"), "photo", "replacement.png" }
            })
            using (HttpRequestMessage photoRequest = Authenticated(
                       HttpMethod.Patch, "/api/admin/schedules/photo", token, photoContent))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(photoRequest)).StatusCode);
            }

            ScheduleResponse photoDetail = await GetScheduleAsync(client, scheduleId);
            string replacementPhoto = Assert.IsType<string>(photoDetail.Photo);
            blobUrls.Add(replacementPhoto);
            Assert.NotEqual(blobUrls[0], replacementPhoto);
            Assert.False(await BlobExistsAsync(blobConnection, blobUrls[0]));
            Assert.True(await BlobExistsAsync(blobConnection, replacementPhoto));

            using (HttpRequestMessage deletePhotoRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/schedules/delete-photo",
                       token,
                       JsonContent.Create(scheduleId)))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deletePhotoRequest)).StatusCode);
            }
            Assert.Null((await GetScheduleAsync(client, scheduleId)).Photo);
            Assert.False(await BlobExistsAsync(blobConnection, replacementPhoto));

            using (HttpRequestMessage deleteRequest = Authenticated(
                       HttpMethod.Delete, $"/api/admin/schedules/{scheduleId}", token))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteRequest)).StatusCode);
            }
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/schedules/{scheduleId}")).StatusCode);
        }
        finally
        {
            await CleanupAsync(dbConnection, blobConnection, marker, scheduleId, blobUrls);
        }
    }

    private static (string DbConnection, string BlobConnection) RequireIntegrationEnvironment()
    {
        if (Environment.GetEnvironmentVariable("FEAT05_INTEGRATION") != "1")
        {
            throw SkipException.ForSkip(
                "Set FEAT05_INTEGRATION=1 with disposable DB_CLOUD and BLOB_STORAGE targets.");
        }

        string dbConnection = Environment.GetEnvironmentVariable("DB_CLOUD")
            ?? throw new InvalidOperationException("DB_CLOUD is required for FEAT-05 integration tests.");
        string blobConnection = Environment.GetEnvironmentVariable("BLOB_STORAGE")
            ?? throw new InvalidOperationException("BLOB_STORAGE is required for FEAT-05 integration tests.");
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

    private static async Task<ScheduleResponse> GetScheduleAsync(HttpClient client, long id)
    {
        HttpResponseMessage response = await client.GetAsync($"/api/schedules/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsType<ScheduleResponse>(await response.Content.ReadFromJsonAsync<ScheduleResponse>());
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
        long scheduleId,
        IEnumerable<string> blobUrls)
    {
        await using (StoronnimVContext context = CreateContext(dbConnection))
        {
            await context.Schedules
                .Where(x => x.Id == scheduleId || x.Title.StartsWith(marker))
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
