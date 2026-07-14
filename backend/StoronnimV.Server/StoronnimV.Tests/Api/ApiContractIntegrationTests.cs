using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StoronnimV.Api.Controllers;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.DTO.Responses.HomePage;
using StoronnimV.Application.DTO.Responses.SchedulePage;
using StoronnimV.Application.DTO.Responses.Video;
using StoronnimV.Application.Exceptions;

namespace StoronnimV.Tests.Api;

public sealed class ApiContractIntegrationTests(AuthApiFactory factory)
    : IClassFixture<AuthApiFactory>
{
    public static TheoryData<HttpMethod, string, object, HttpStatusCode> JsonRequestMatrix => new()
    {
        { HttpMethod.Post, "/api/admin/socials", new { memberId = 1, url = "https://example.test", type = "Website" }, HttpStatusCode.Created },
        { HttpMethod.Patch, "/api/admin/news", new { id = 1, title = "Title", description = "Description", priority = "Main", date = "2026-07-14" }, HttpStatusCode.NoContent },
        { HttpMethod.Patch, "/api/admin/schedules", new { id = 1, title = "Title", performanceDateTime = "2026-07-14T19:30", description = "Description", location = "Location" }, HttpStatusCode.NoContent },
        { HttpMethod.Patch, "/api/admin/videos", new { id = 1, title = "Title", type = "Performance" }, HttpStatusCode.NoContent },
        { HttpMethod.Patch, "/api/admin/group-pages", new { id = 1, description = "Description" }, HttpStatusCode.NoContent },
        { HttpMethod.Patch, "/api/admin/group-pages/members", new { id = 1, fullName = "Member", description = "Description", role = "Role" }, HttpStatusCode.NoContent },
        { HttpMethod.Patch, "/api/admin/music-platforms", new { id = 1, platformUrl = "https://example.test" }, HttpStatusCode.NoContent },
        { HttpMethod.Patch, "/api/admin/socials", new { id = 1, url = "https://example.test", type = "Website" }, HttpStatusCode.NoContent },
        { HttpMethod.Patch, "/api/admin/group-socials", new { id = 1, linkUrl = "https://example.test" }, HttpStatusCode.NoContent },
        { HttpMethod.Patch, "/api/admin/news/video", new { id = 1, videoId = 2 }, HttpStatusCode.NoContent }
    };

    [Theory]
    [MemberData(nameof(JsonRequestMatrix))]
    public async Task BodyBoundAdminEndpoint_AcceptsJson(
        HttpMethod method, string route, object body, HttpStatusCode expectedStatus)
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);
        using HttpRequestMessage request = new(method, route)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", factory.CreateToken("Basic"));

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(expectedStatus, response.StatusCode);
    }

    [Fact]
    public async Task FormBoundNewsEndpoint_AcceptsIsoDate()
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);
        using MultipartFormDataContent content = new()
        {
            { new StringContent("Title"), "title" },
            { new StringContent("Description"), "description" },
            { new StringContent("Main"), "priority" },
            { new StringContent("2026-07-14"), "date" }
        };
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/admin/news") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", factory.CreateToken("Basic"));

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task FormBoundScheduleEndpoint_AcceptsIsoDateTime()
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);
        using MultipartFormDataContent content = new()
        {
            { new StringContent("Title"), "title" },
            { new StringContent("2026-07-14T19:30"), "performanceDateTime" },
            { new StringContent("Description"), "description" },
            { new StringContent("Location"), "location" },
            { new StringContent("Active"), "status" }
        };
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/admin/schedules") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", factory.CreateToken("Basic"));

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory]
    [InlineData("not-a-date")]
    [InlineData("")]
    public async Task FormBoundNewsEndpoint_WithInvalidDate_ReturnsValidationProblem(string date)
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);
        using MultipartFormDataContent content = new()
        {
            { new StringContent("Title"), "title" },
            { new StringContent("Description"), "description" },
            { new StringContent("Main"), "priority" },
            { new StringContent(date), "date" }
        };
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/admin/news") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", factory.CreateToken("Basic"));

        HttpResponseMessage response = await client.SendAsync(request);
        ApiErrorResponse problem = AssertProblem(
            await response.Content.ReadFromJsonAsync<ApiErrorResponse>(),
            HttpStatusCode.BadRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(problem.Errors);
    }

    [Fact]
    public async Task FormBoundScheduleEndpoint_WithInvalidDate_ReturnsValidationProblem()
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);
        using MultipartFormDataContent content = new()
        {
            { new StringContent("Title"), "title" },
            { new StringContent("not-a-date"), "performanceDateTime" },
            { new StringContent("Description"), "description" },
            { new StringContent("Location"), "location" },
            { new StringContent("Active"), "status" }
        };
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/admin/schedules") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", factory.CreateToken("Basic"));

        HttpResponseMessage response = await client.SendAsync(request);
        ApiErrorResponse problem = AssertProblem(
            await response.Content.ReadFromJsonAsync<ApiErrorResponse>(),
            HttpStatusCode.BadRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(problem.Errors);
    }

    [Fact]
    public async Task FormBoundNewsEndpoint_WithoutDate_ReturnsValidationProblem()
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);
        using MultipartFormDataContent content = new()
        {
            { new StringContent("Title"), "title" },
            { new StringContent("Description"), "description" },
            { new StringContent("Main"), "priority" }
        };
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/admin/news") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", factory.CreateToken("Basic"));

        HttpResponseMessage response = await client.SendAsync(request);
        ApiErrorResponse problem = AssertProblem(
            await response.Content.ReadFromJsonAsync<ApiErrorResponse>(),
            HttpStatusCode.BadRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(problem.Errors);
    }

    [Fact]
    public async Task FormBoundScheduleEndpoint_WithoutDate_ReturnsValidationProblem()
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);
        using MultipartFormDataContent content = new()
        {
            { new StringContent("Title"), "title" },
            { new StringContent("Description"), "description" },
            { new StringContent("Location"), "location" },
            { new StringContent("Active"), "status" }
        };
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/admin/schedules") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", factory.CreateToken("Basic"));

        HttpResponseMessage response = await client.SendAsync(request);
        ApiErrorResponse problem = AssertProblem(
            await response.Content.ReadFromJsonAsync<ApiErrorResponse>(),
            HttpStatusCode.BadRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(problem.Errors);
    }

    [Theory]
    [InlineData("not-found", HttpStatusCode.NotFound)]
    [InlineData("unsupported-media", HttpStatusCode.UnsupportedMediaType)]
    [InlineData("server", HttpStatusCode.InternalServerError)]
    public async Task ExceptionPaths_ReturnUnifiedProblem(string kind, HttpStatusCode expectedStatus)
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);

        HttpResponseMessage response = await client.GetAsync($"/api/contract-test/errors/{kind}");
        ApiErrorResponse problem = AssertProblem(
            await response.Content.ReadFromJsonAsync<ApiErrorResponse>(),
            expectedStatus);

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Empty(problem.Errors);
        if (expectedStatus == HttpStatusCode.InternalServerError)
        {
            Assert.DoesNotContain("sensitive server detail", problem.Detail, StringComparison.Ordinal);
        }
    }

    [Theory]
    [InlineData("/api/admin/isAdmin", null, HttpStatusCode.Unauthorized)]
    [InlineData("/api/super-admin/basic-admins", "Basic", HttpStatusCode.Forbidden)]
    [InlineData("/api/route-that-does-not-exist", null, HttpStatusCode.NotFound)]
    public async Task EmptyFrameworkErrors_ReturnUnifiedProblem(
        string route, string? role, HttpStatusCode expectedStatus)
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);
        using HttpRequestMessage request = new(HttpMethod.Get, route);
        if (role is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer", factory.CreateToken(role));
        }

        HttpResponseMessage response = await client.SendAsync(request);
        ApiErrorResponse problem = AssertProblem(
            await response.Content.ReadFromJsonAsync<ApiErrorResponse>(),
            expectedStatus);

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Empty(problem.Errors);
    }

    [Fact]
    public void PublicDtoContract_ExposesScheduleStatusAndNullableMedia()
    {
        PropertyInfo? status = typeof(ScheduleShortResponse).GetProperty("Status");
        NullabilityInfoContext nullability = new();

        Assert.NotNull(status);
        Assert.Equal(NullabilityState.Nullable,
            nullability.Create(typeof(NewsHomeResponse).GetProperty("Photo")!).ReadState);
        Assert.Equal(NullabilityState.Nullable,
            nullability.Create(typeof(ScheduleHomeResponse).GetProperty("Photo")!).ReadState);
    }

    [Theory]
    [InlineData("/api/home/schedule")]
    [InlineData("/api/home/video")]
    public async Task NullableHomeEndpoint_WithoutEntity_ReturnsJsonNull(string route)
    {
        using WebApplicationFactory<AccountController> app = CreateContractApp();
        using HttpClient client = CreateClient(app);

        HttpResponseMessage response = await client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("null", await response.Content.ReadAsStringAsync());
    }

    private WebApplicationFactory<AccountController> CreateContractApp()
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdminControllerService, StubAdminControllerService>();
                services.AddScoped<IHomeControllerService, StubHomeControllerService>();
            });
        });
    }

    private static HttpClient CreateClient(WebApplicationFactory<AccountController> app)
    {
        return app.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = false
        });
    }

    private static ApiErrorResponse AssertProblem(ApiErrorResponse? problem, HttpStatusCode expectedStatus)
    {
        ApiErrorResponse payload = Assert.IsType<ApiErrorResponse>(problem);
        Assert.Equal((int)expectedStatus, payload.Status);
        Assert.False(string.IsNullOrWhiteSpace(payload.Title));
        Assert.False(string.IsNullOrWhiteSpace(payload.Detail));
        return payload;
    }

    private sealed class StubAdminControllerService : IAdminControllerService
    {
        public Task DeleteNewsItemAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteScheduleAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteVideoAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteGroupPageAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteMemberAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteMusicPlatformAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteSocialAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteGroupSocialAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task AddNewsItemAsync(NewsItemAdditionRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task AddScheduleAsync(ScheduleAdditionRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task AddVideoAsync(VideoAdditionRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task AddGroupPageAsync(GroupPageAdditionRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task AddMemberAsync(MemberAdditionRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task AddMusicPlatformAsync(MusicPlatformAdditionRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task AddSocialAsync(SocialAdditionRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task AddGroupSocialAsync(GroupSocialAdditionRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateNewsItemAsync(NewsItemEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateScheduleAsync(ScheduleEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateVideoAsync(VideoEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateGroupPageAsync(GroupPageEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateMemberAsync(MemberEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateMusicPlatformAsync(MusicPlatformEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateSocialAsync(SocialEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateGroupSocialAsync(GroupSocialEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateNewsItemPhotoAsync(PhotoEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteNewsItemPhotoAsync(long id, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateSchedulePhotoAsync(PhotoEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateGroupPagePhotoAsync(PhotoEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateMemberPhotoAsync(PhotoEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateMusicPlatformPhotoAsync(PhotoEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task UpdateNewsItemVideoAsync(EntityVideoEditRequest request, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteNewsItemVideoAsync(long id, CancellationToken ct) => Task.CompletedTask;
    }

    private sealed class StubHomeControllerService : IHomeControllerService
    {
        public Task<IEnumerable<NewsHomeResponse>> GetMainNewsAsync(int count, CancellationToken ct) =>
            Task.FromResult<IEnumerable<NewsHomeResponse>>([]);

        public Task<ScheduleHomeResponse?> GetNearestScheduleAsync(CancellationToken ct) =>
            Task.FromResult<ScheduleHomeResponse?>(null);

        public Task<VideoPageResponse?> GetPromotionVideoAsync(CancellationToken ct) =>
            Task.FromResult<VideoPageResponse?>(null);
    }
}

public sealed record ApiErrorResponse(
    int Status,
    string Title,
    string Detail,
    Dictionary<string, string[]> Errors);

[AllowAnonymous]
[Route("api/contract-test/errors")]
[ApiController]
public sealed class ErrorContractProbeController : ControllerBase
{
    [HttpGet("{kind}")]
    public IActionResult Throw(string kind)
    {
        return kind switch
        {
            "not-found" => throw new EntityNotFoundException("Missing test entity."),
            "unsupported-media" => throw new PhotoResizingException("Unsupported test media."),
            "server" => throw new InvalidOperationException("sensitive server detail"),
            _ => throw new ArgumentException("Unknown error kind.", nameof(kind))
        };
    }
}
