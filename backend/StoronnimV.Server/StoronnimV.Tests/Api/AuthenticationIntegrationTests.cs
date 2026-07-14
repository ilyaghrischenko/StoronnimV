using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StoronnimV.Api.Controllers;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Identity;
using StoronnimV.Application.DTO.Requests.Account;
using StoronnimV.Application.DTO.Requests.Entities.Admin;
using StoronnimV.Application.DTO.Responses.Admin;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;

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

    [Theory]
    [InlineData("Basic", TokenTransport.AuthorizationHeader)]
    [InlineData("Basic", TokenTransport.Cookie)]
    [InlineData("SuperAdmin", TokenTransport.AuthorizationHeader)]
    [InlineData("SuperAdmin", TokenTransport.Cookie)]
    public async Task AdminRoleEndpoint_WithToken_ReturnsServerRole(string role, TokenTransport transport)
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Get, "/api/admin/role");
        AddToken(request, factory.CreateToken(role), transport);

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(role, await response.Content.ReadAsStringAsync());
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
        string authCookie = $"Token={factory.CreateToken("Basic")}";
        (string antiforgeryCookie, string requestToken) = await GetAntiforgeryTokenAsync(client, authCookie);
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/admin/logout");
        request.Headers.Add("Cookie", $"{authCookie}; {antiforgeryCookie}");
        request.Headers.Add("X-CSRF-TOKEN", requestToken);

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CredentialedLoginAndMutation_WithAntiforgeryToken_Succeed()
    {
        using HttpClient client = factory.CreateApiClient();
        (string anonymousCookie, string anonymousToken) = await GetAntiforgeryTokenAsync(client);
        using HttpRequestMessage loginRequest = new(HttpMethod.Post, "/api/account/login")
        {
            Content = JsonContent.Create(new LogInRequest
            {
                Login = "test-admin",
                Password = "test-password"
            })
        };
        loginRequest.Headers.Add("Cookie", anonymousCookie);
        loginRequest.Headers.Add("Origin", "https://client.test");
        loginRequest.Headers.Add("X-CSRF-TOKEN", anonymousToken);

        HttpResponseMessage loginResponse = await client.SendAsync(loginRequest);
        string authCookie = GetCookie(loginResponse, "Token");

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Contains("HttpOnly", GetSetCookie(loginResponse, "Token"), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SameSite=Lax", GetSetCookie(loginResponse, "Token"), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Domain=", GetSetCookie(loginResponse, "Token"), StringComparison.OrdinalIgnoreCase);

        (string antiforgeryCookie, string requestToken) = await GetAntiforgeryTokenAsync(client, authCookie);
        using HttpRequestMessage mutationRequest = new(HttpMethod.Post, "/api/auth-test/mutation");
        mutationRequest.Headers.Add("Cookie", $"{authCookie}; {antiforgeryCookie}");
        mutationRequest.Headers.Add("Origin", "https://client.test");
        mutationRequest.Headers.Add("X-CSRF-TOKEN", requestToken);

        HttpResponseMessage mutationResponse = await client.SendAsync(mutationRequest);

        Assert.Equal(HttpStatusCode.OK, mutationResponse.StatusCode);
    }

    [Fact]
    public async Task CookieMutation_WithoutAntiforgeryToken_IsRejected()
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/auth-test/mutation");
        request.Headers.Add("Cookie", $"Token={factory.CreateToken("Basic")}");
        request.Headers.Add("Origin", "https://attacker.test");

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task CookieMutation_WithInvalidToken_ReturnsUnauthorized()
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/auth-test/mutation");
        request.Headers.Add("Cookie", "Token=not-a-jwt");

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task BearerMutation_WithoutAntiforgeryToken_Succeeds()
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Post, "/api/auth-test/mutation");
        AddToken(request, factory.CreateToken("Basic"), TokenTransport.AuthorizationHeader);

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("https://client.test", true)]
    [InlineData("https://attacker.test", false)]
    public async Task CorsPreflight_AllowsOnlyConfiguredOrigin(string origin, bool allowed)
    {
        using HttpClient client = factory.CreateApiClient();
        using HttpRequestMessage request = new(HttpMethod.Options, "/api/auth-test/mutation");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "X-CSRF-TOKEN");

        HttpResponseMessage response = await client.SendAsync(request);
        bool hasAllowedOrigin = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(allowed, hasAllowedOrigin && origins?.Contains(origin) == true);
    }

    private static async Task<(string Cookie, string Token)> GetAntiforgeryTokenAsync(
        HttpClient client, string? authCookie = null)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, "/api/account/csrf-token");
        if (authCookie is not null)
        {
            request.Headers.Add("Cookie", authCookie);
        }

        HttpResponseMessage response = await client.SendAsync(request);
        AntiforgeryTokenResponse? payload =
            await response.Content.ReadFromJsonAsync<AntiforgeryTokenResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        return (GetCookie(response, ".AspNetCore.Antiforgery"), payload.RequestToken);
    }

    private static string GetCookie(HttpResponseMessage response, string name)
    {
        return GetSetCookie(response, name).Split(';', 2)[0];
    }

    private static string GetSetCookie(HttpResponseMessage response, string name)
    {
        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var values));
        return Assert.Single(values, value => value.StartsWith(name, StringComparison.Ordinal));
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
        Environment.SetEnvironmentVariable("BLOB_STORAGE", "UseDevelopmentStorage=true");
    }

    public HttpClient CreateApiClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = false
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
            services.AddScoped<IAccountService, StubAccountService>();
            services.AddScoped<ISuperAdminControllerService, StubSuperAdminControllerService>();
        });
    }

    private sealed class StubAccountService : IAccountService
    {
        public Task<Admin> LogInAsync(string login, string password, CancellationToken ct)
        {
            return Task.FromResult(new Admin
            {
                Id = 1,
                Login = login,
                Password = string.Empty,
                Type = AdminType.Basic
            });
        }
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

public sealed record AntiforgeryTokenResponse(string RequestToken);

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/auth-test/mutation")]
[ApiController]
public sealed class AuthMutationProbeController : ControllerBase
{
    [HttpPost]
    public IActionResult Mutate() => Ok();
}
