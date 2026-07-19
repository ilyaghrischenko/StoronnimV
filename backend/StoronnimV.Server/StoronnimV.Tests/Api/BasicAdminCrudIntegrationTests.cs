using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StoronnimV.Api.Controllers;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Identity;
using StoronnimV.Application.DTO.Responses.Admin;
using StoronnimV.Application.Services.Controllers;
using StoronnimV.Application.Services.Identity;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Infrastructure;
using Xunit.Sdk;

namespace StoronnimV.Tests.Api;

public sealed class BasicAdminCrudIntegrationTests(AuthApiFactory factory)
    : IClassFixture<AuthApiFactory>
{
    [Fact]
    public async Task AdminLoginDatabase_RejectsDuplicateRows()
    {
        string dbConnection = RequireIntegrationEnvironment();
        string login = $"feat09duplicate{Guid.NewGuid():N}";

        try
        {
            await using StoronnimVContext context = CreateContext(dbConnection);
            context.Admins.Add(new Admin
            {
                Login = login,
                Password = "first-hash",
                Type = AdminType.Basic
            });
            await context.SaveChangesAsync();

            context.Admins.Add(new Admin
            {
                Login = login,
                Password = "second-hash",
                Type = AdminType.SuperAdmin
            });

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }
        finally
        {
            await using StoronnimVContext cleanupContext = CreateContext(dbConnection);
            await cleanupContext.Admins.Where(admin => admin.Login == login).ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task BasicAdminCrud_RealApiPostgres_EnforcesRoleAndAccountTypeBoundaries()
    {
        string dbConnection = RequireIntegrationEnvironment();
        string marker = $"feat09{Guid.NewGuid():N}";
        string superLogin = $"{marker}owner1";
        string concurrentLogin = $"{marker}race1";
        string initialLogin = $"{marker}admin1";
        string editedLogin = $"{marker}admin2";
        const string initialPassword = "AAAaa11111";
        const string editedPassword = "BBBbb22222";

        try
        {
            long superAdminId = await AddSuperAdminAsync(dbConnection, superLogin, initialPassword);
            using WebApplicationFactory<AccountController> app = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<ISuperAdminControllerService, SuperAdminControllerService>();
                    services.AddScoped<IAccountService, AccountService>();
                });
            });
            using HttpClient client = CreateClient(app);
            string basicToken = factory.CreateToken("Basic");
            string superToken = factory.CreateToken("SuperAdmin");

            using (HttpRequestMessage basicListRequest = Authenticated(
                       HttpMethod.Get, "/api/super-admin/basic-admins", basicToken))
            {
                Assert.Equal(HttpStatusCode.Forbidden, (await client.SendAsync(basicListRequest)).StatusCode);
            }

            using (HttpRequestMessage firstConcurrentCreate = Authenticated(
                       HttpMethod.Post,
                       "/api/super-admin/basic-admins",
                       superToken,
                       JsonContent.Create(new { login = concurrentLogin, password = initialPassword })))
            using (HttpRequestMessage secondConcurrentCreate = Authenticated(
                       HttpMethod.Post,
                       "/api/super-admin/basic-admins",
                       superToken,
                       JsonContent.Create(new { login = concurrentLogin, password = initialPassword })))
            {
                HttpResponseMessage[] responses = await Task.WhenAll(
                    client.SendAsync(firstConcurrentCreate),
                    client.SendAsync(secondConcurrentCreate));
                Assert.Equal(1, responses.Count(response => response.StatusCode == HttpStatusCode.OK));
                Assert.Equal(1, responses.Count(response => response.StatusCode == HttpStatusCode.BadRequest));
                foreach (HttpResponseMessage response in responses)
                {
                    response.Dispose();
                }
            }

            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Assert.Equal(1, await context.Admins.CountAsync(admin => admin.Login == concurrentLogin));
            }

            BasicAdminResponse created;
            using (HttpRequestMessage createRequest = Authenticated(
                       HttpMethod.Post,
                       "/api/super-admin/basic-admins",
                       superToken,
                       JsonContent.Create(new { login = initialLogin, password = initialPassword })))
            {
                HttpResponseMessage response = await client.SendAsync(createRequest);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                created = Assert.IsType<BasicAdminResponse>(
                    await response.Content.ReadFromJsonAsync<BasicAdminResponse>());
            }

            Assert.Equal(initialLogin, created.Login);
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Admin stored = await context.Admins.AsNoTracking().SingleAsync(admin => admin.Id == created.Id);
                Assert.Equal(AdminType.Basic, stored.Type);
                Assert.NotEqual(initialPassword, stored.Password);
            }

            IReadOnlyList<BasicAdminResponse> list = await GetBasicAdminsAsync(client, superToken);
            Assert.Contains(list, admin => admin.Id == created.Id && admin.Login == initialLogin);
            Assert.DoesNotContain(list, admin => admin.Id == superAdminId);

            using (HttpRequestMessage editLoginRequest = Authenticated(
                       HttpMethod.Patch,
                       $"/api/super-admin/basic-admins/{created.Id}/login",
                       superToken,
                       JsonContent.Create(new { newLogin = editedLogin })))
            {
                HttpResponseMessage response = await client.SendAsync(editLoginRequest);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                BasicAdminResponse edited = Assert.IsType<BasicAdminResponse>(
                    await response.Content.ReadFromJsonAsync<BasicAdminResponse>());
                Assert.Equal(editedLogin, edited.Login);
            }

            using (HttpRequestMessage editPasswordRequest = Authenticated(
                       HttpMethod.Patch,
                       $"/api/super-admin/basic-admins/{created.Id}/password",
                       superToken,
                       JsonContent.Create(new
                       {
                           oldPassword = initialPassword,
                           newPassword = editedPassword
                       })))
            {
                Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(editPasswordRequest)).StatusCode);
            }

            Assert.Equal("Basic", await LogInAsync(client, editedLogin, editedPassword));

            foreach (HttpRequestMessage request in new[]
                     {
                         Authenticated(
                             HttpMethod.Patch,
                             $"/api/super-admin/basic-admins/{superAdminId}/login",
                             superToken,
                             JsonContent.Create(new { newLogin = $"{marker}blocked1" })),
                         Authenticated(
                             HttpMethod.Patch,
                             $"/api/super-admin/basic-admins/{superAdminId}/password",
                             superToken,
                             JsonContent.Create(new
                             {
                                 oldPassword = initialPassword,
                                 newPassword = editedPassword
                             })),
                         Authenticated(
                             HttpMethod.Delete,
                             $"/api/super-admin/basic-admins/{superAdminId}",
                             superToken)
                     })
            {
                using (request)
                {
                    Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(request)).StatusCode);
                }
            }

            using (HttpRequestMessage duplicateSuperLoginRequest = Authenticated(
                       HttpMethod.Post,
                       "/api/super-admin/basic-admins",
                       superToken,
                       JsonContent.Create(new { login = superLogin, password = initialPassword })))
            {
                Assert.Equal(
                    HttpStatusCode.BadRequest,
                    (await client.SendAsync(duplicateSuperLoginRequest)).StatusCode);
            }

            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Admin superAdmin = await context.Admins.AsNoTracking().SingleAsync(admin => admin.Id == superAdminId);
                Assert.Equal(superLogin, superAdmin.Login);
                Assert.Equal(AdminType.SuperAdmin, superAdmin.Type);
            }

            using (HttpRequestMessage deleteRequest = Authenticated(
                       HttpMethod.Delete,
                       $"/api/super-admin/basic-admins/{created.Id}",
                       superToken))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteRequest)).StatusCode);
            }

            Assert.DoesNotContain(
                await GetBasicAdminsAsync(client, superToken),
                admin => admin.Id == created.Id);
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Assert.False(await context.Admins.AnyAsync(admin => admin.Id == created.Id));
            }
        }
        finally
        {
            await using StoronnimVContext context = CreateContext(dbConnection);
            await context.Admins.Where(admin => admin.Login.StartsWith(marker)).ExecuteDeleteAsync();
        }
    }

    private static string RequireIntegrationEnvironment()
    {
        if (Environment.GetEnvironmentVariable("FEAT09_INTEGRATION") != "1")
        {
            throw SkipException.ForSkip(
                "Set FEAT09_INTEGRATION=1 with a disposable DB_CLOUD target.");
        }

        return Environment.GetEnvironmentVariable("DB_CLOUD")
               ?? throw new InvalidOperationException("DB_CLOUD is required for FEAT-09 integration tests.");
    }

    private static async Task<long> AddSuperAdminAsync(
        string dbConnection,
        string login,
        string password)
    {
        await using StoronnimVContext context = CreateContext(dbConnection);
        PasswordHasher<Admin> passwordHasher = new();
        Admin superAdmin = new()
        {
            Login = login,
            Password = string.Empty,
            Type = AdminType.SuperAdmin
        };
        superAdmin.Password = passwordHasher.HashPassword(superAdmin, password);
        context.Admins.Add(superAdmin);
        await context.SaveChangesAsync();
        return superAdmin.Id;
    }

    private static HttpClient CreateClient(WebApplicationFactory<AccountController> app) =>
        app.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = false
        });

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

    private static async Task<IReadOnlyList<BasicAdminResponse>> GetBasicAdminsAsync(
        HttpClient client,
        string token)
    {
        using HttpRequestMessage request = Authenticated(
            HttpMethod.Get, "/api/super-admin/basic-admins", token);
        HttpResponseMessage response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsAssignableFrom<IReadOnlyList<BasicAdminResponse>>(
            await response.Content.ReadFromJsonAsync<List<BasicAdminResponse>>());
    }

    private static async Task<string> LogInAsync(HttpClient client, string login, string password)
    {
        using HttpRequestMessage tokenRequest = new(HttpMethod.Get, "/api/account/csrf-token");
        HttpResponseMessage tokenResponse = await client.SendAsync(tokenRequest);
        AntiforgeryTokenResponse token = Assert.IsType<AntiforgeryTokenResponse>(
            await tokenResponse.Content.ReadFromJsonAsync<AntiforgeryTokenResponse>());
        string antiforgeryCookie = GetCookie(tokenResponse, ".AspNetCore.Antiforgery");

        using HttpRequestMessage loginRequest = new(HttpMethod.Post, "/api/account/login")
        {
            Content = JsonContent.Create(new { login, password })
        };
        loginRequest.Headers.Add("Cookie", antiforgeryCookie);
        loginRequest.Headers.Add("Origin", "https://client.test");
        loginRequest.Headers.Add("X-CSRF-TOKEN", token.RequestToken);
        HttpResponseMessage loginResponse = await client.SendAsync(loginRequest);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        return await loginResponse.Content.ReadAsStringAsync();
    }

    private static string GetCookie(HttpResponseMessage response, string name)
    {
        Assert.True(response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? values));
        string value = Assert.Single(values, item => item.StartsWith(name, StringComparison.Ordinal));
        return value.Split(';', 2)[0];
    }

    private static StoronnimVContext CreateContext(string connectionString)
    {
        DbContextOptions<StoronnimVContext> options = new DbContextOptionsBuilder<StoronnimVContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new StoronnimVContext(options);
    }

    private sealed record AntiforgeryTokenResponse(string RequestToken);
}
