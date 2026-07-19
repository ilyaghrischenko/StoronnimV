using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using StoronnimV.Api.Controllers;
using Xunit.Sdk;

namespace StoronnimV.Tests.Api;

public sealed class HangfireDashboardIntegrationTests
{
    [Fact]
    public async Task Production_DoesNotExposeHangfireDashboard()
    {
        if (Environment.GetEnvironmentVariable("API04_INTEGRATION") != "1")
        {
            throw SkipException.ForSkip(
                "Set API04_INTEGRATION=1 with a disposable DB_CLOUD target.");
        }

        using ProductionApiFactory factory = new();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        HttpResponseMessage response = await client.GetAsync("/hangfire");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class ProductionApiFactory : WebApplicationFactory<AccountController>
    {
        public ProductionApiFactory()
        {
            Environment.SetEnvironmentVariable("TOKEN_ISSUER", "StoronnimV.AuthTests");
            Environment.SetEnvironmentVariable("TOKEN_AUDIENCE", "StoronnimV.AuthTests.Client");
            Environment.SetEnvironmentVariable("TOKEN_KEY", new string('t', 64));
            Environment.SetEnvironmentVariable("TOKEN_LIFETIME", "1");
            Environment.SetEnvironmentVariable("CLIENT_URL", "https://client.test");
            Environment.SetEnvironmentVariable("BLOB_STORAGE", "UseDevelopmentStorage=true");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");
        }
    }
}
