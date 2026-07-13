using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StoronnimV.Api.Controllers;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Requests.Entities.Admin;
using StoronnimV.Application.DTO.Responses.Admin;

namespace StoronnimV.Tests.Api;

public sealed class AuthenticationIntegrationTests(AuthApiFactory factory)
    : IClassFixture<AuthApiFactory>
{
    [Fact]
    public void AuthenticationMiddleware_IsExplicitlyBeforeAuthorization()
    {
        string testDirectory = Path.GetDirectoryName(GetSourceFilePath())!;
        string programPath = Path.GetFullPath(Path.Combine(
            testDirectory, "..", "..", "StoronnimV.Api", "Program.cs"));
        string programSource = File.ReadAllText(programPath);
        int authenticationIndex = programSource.IndexOf("app.UseAuthentication();", StringComparison.Ordinal);
        int authorizationIndex = programSource.IndexOf("app.UseAuthorization();", StringComparison.Ordinal);

        Assert.True(authenticationIndex >= 0, "Program.cs must call app.UseAuthentication().");
        Assert.True(authenticationIndex < authorizationIndex,
            "app.UseAuthentication() must run before app.UseAuthorization().");
    }

    [Fact]
    public async Task AdminEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        using HttpClient client = factory.CreateApiClient();

        HttpResponseMessage response = await client.GetAsync("/api/admin/isAdmin");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(TokenTransport.AuthorizationHeader)]
    [InlineData(TokenTransport.Cookie)]
    public async Task PublicProbe_WithToken_FormsExpectedPrincipal(TokenTransport transport)
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Get, "/api/auth-test/principal");
        AddToken(request, factory.CreateToken("Basic"), transport);

        HttpResponseMessage response = await client.SendAsync(request);
        AuthProbeResponse? principal = await response.Content.ReadFromJsonAsync<AuthProbeResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(principal);
        Assert.True(principal.IsAuthenticated);
        Assert.Equal("1", principal.Name);
        Assert.Equal("Basic", principal.Role);
    }

    [Theory]
    [InlineData(TokenTransport.AuthorizationHeader)]
    [InlineData(TokenTransport.Cookie)]
    public async Task AdminEndpoint_WithBasicToken_FormsPrincipal(TokenTransport transport)
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Get, "/api/admin/isAdmin");
        AddToken(request, factory.CreateToken("Basic"), transport);

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await response.Content.ReadFromJsonAsync<bool>());
    }

    [Fact]
    public async Task SuperAdminEndpoint_WithBasicToken_ReturnsForbidden()
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Get, "/api/super-admin/basic-admins");
        AddToken(request, factory.CreateToken("Basic"), TokenTransport.AuthorizationHeader);

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SuperAdminEndpoint_WithSuperAdminToken_ReturnsOk()
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Get, "/api/super-admin/basic-admins");
        AddToken(request, factory.CreateToken("SuperAdmin"), TokenTransport.AuthorizationHeader);

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AdminEndpoint_WithInvalidOrExpiredToken_ReturnsUnauthorized(bool expired)
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Get, "/api/admin/isAdmin");
        string token = expired ? factory.CreateToken("Basic", expired: true) : "not-a-jwt";
        AddToken(request, token, TokenTransport.AuthorizationHeader);

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task LogoutEndpoint_WithBasicToken_ReturnsOk()
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/admin/logout");
        AddToken(request, factory.CreateToken("Basic"), TokenTransport.Cookie);

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static void AddToken(HttpRequestMessage request, string token, TokenTransport transport)
    {
        if (transport == TokenTransport.Cookie)
        {
            request.Headers.Add("Cookie", $"Token={token}");
            return;
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static string GetSourceFilePath([CallerFilePath] string path = "") => path;
}

public enum TokenTransport
{
    AuthorizationHeader,
    Cookie
}

public sealed class AuthApiFactory : WebApplicationFactory<AccountController>
{
    private const string Issuer = "StoronnimV.AuthTests";
    private const string Audience = "StoronnimV.AuthTests.Client";
    private static readonly string SigningKey = new('t', 64);

    public AuthApiFactory()
    {
        Environment.SetEnvironmentVariable("TOKEN_ISSUER", Issuer);
        Environment.SetEnvironmentVariable("TOKEN_AUDIENCE", Audience);
        Environment.SetEnvironmentVariable("TOKEN_KEY", SigningKey);
        Environment.SetEnvironmentVariable("TOKEN_LIFETIME", "1");
        Environment.SetEnvironmentVariable("CLIENT_URL", "https://client.test");
        Environment.SetEnvironmentVariable("DOMAIN", "localhost");
        Environment.SetEnvironmentVariable("BLOB_STORAGE", "UseDevelopmentStorage=true");
    }

    public HttpClient CreateApiClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }

    public string CreateToken(string role, bool expired = false)
    {
        DateTime now = DateTime.UtcNow;
        DateTime notBefore = expired ? now.AddHours(-2) : now.AddMinutes(-1);
        DateTime expires = expired ? now.AddHours(-1) : now.AddHours(1);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "1"),
            new Claim(ClaimTypes.Role, role)
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(Issuer, Audience, claims, notBefore, expires, credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddControllers().AddApplicationPart(typeof(AuthProbeController).Assembly);
            services.AddScoped<ISuperAdminControllerService, StubSuperAdminControllerService>();
        });
    }

    private sealed class StubSuperAdminControllerService : ISuperAdminControllerService
    {
        public Task<IEnumerable<BasicAdminResponse>> GetAllAsync(CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<BasicAdminResponse>>([]);
        }

        public Task DeleteBasicAdminAsync(long id, CancellationToken ct) => throw new NotSupportedException();

        public Task<BasicAdminResponse> AddBasicAdminAsync(CreateBasicAdminRequest request, CancellationToken ct) =>
            throw new NotSupportedException();

        public Task EditBasicAdminPasswordAsync(long id, EditBasicAdminPasswordRequest passwordRequest,
            CancellationToken ct) => throw new NotSupportedException();

        public Task<BasicAdminResponse> EditBasicAdminLoginAsync(long id, EditBasicAdminLoginRequest request,
            CancellationToken ct) => throw new NotSupportedException();
    }
}

[AllowAnonymous]
[Route("api/auth-test/principal")]
[ApiController]
public sealed class AuthProbeController : ControllerBase
{
    [HttpGet]
    public ActionResult<AuthProbeResponse> GetPrincipal()
    {
        return Ok(new AuthProbeResponse(
            User.Identity?.IsAuthenticated == true,
            User.Identity?.Name,
            User.FindFirst(ClaimTypes.Role)?.Value));
    }
}

public sealed record AuthProbeResponse(bool IsAuthenticated, string? Name, string? Role);
