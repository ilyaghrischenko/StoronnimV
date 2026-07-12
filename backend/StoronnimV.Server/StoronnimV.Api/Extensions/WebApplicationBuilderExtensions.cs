using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.Contracts.Home;
using StoronnimV.Application.Contracts.Identity;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.Mapping.Group;
using StoronnimV.Application.Mapping.Home;
using StoronnimV.Application.Mapping.News;
using StoronnimV.Application.Mapping.Schedule;
using StoronnimV.Application.Options;
using StoronnimV.Application.Services.Background;
using StoronnimV.Application.Services.Controllers;
using StoronnimV.Application.Services.Entities;
using StoronnimV.Application.Services.Home;
using StoronnimV.Application.Services.Identity;
using StoronnimV.Application.Services.Utils;
using StoronnimV.Application.Validation.Admin;
using StoronnimV.Infrastructure;
using StoronnimV.Domain.Contracts.AzureBlobStorage;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Infrastructure.Repositories.AzureBlobStorage;
using StoronnimV.Infrastructure.Repositories.Database;
using StoronnimV.Infrastructure.Repositories.Database.Shared;

namespace StoronnimV.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<INewsService, NewsService>();
        builder.Services.AddScoped<IScheduleService, ScheduleService>();
        builder.Services.AddScoped<ISocialService, SocialService>();
        builder.Services.AddScoped<IGroupSocialService, GroupSocialService>();
        builder.Services.AddScoped<IMemberService, MemberService>();
        builder.Services.AddScoped<IGroupPageService, GroupPageService>();
        builder.Services.AddScoped<IMusicPlatformService, MusicPlatformService>();
        builder.Services.AddScoped<IVideoService, VideoService>();
        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();

        builder.Services.AddScoped<INewsControllerService, NewsControllerService>();
        builder.Services.AddScoped<ISchedulesControllerService, SchedulesControllerService>();
        builder.Services.AddScoped<IGroupPageControllerService, GroupPageControllerService>();
        builder.Services.AddScoped<IGroupSocialsControllerService, GroupSocialsControllerService>();
        builder.Services.AddScoped<IMusicControllerService, MusicControllerService>();
        builder.Services.AddScoped<IVideoControllerService, VideoControllerService>();
        builder.Services.AddScoped<IHomeControllerService, HomeControllerService>();
        builder.Services.AddScoped<IAccountControllerService, AccountControllerService>();
        builder.Services.AddScoped<IAdminControllerService, AdminControllerService>();
        builder.Services.AddScoped<ISuperAdminControllerService, SuperAdminControllerService>();

        builder.Services.AddScoped<IHomeService, HomeService>();
        builder.Services.AddScoped<IJwtBearerService, JwtBearerService>();
        builder.Services.AddScoped<IImageResizerService, ImageResizerService>();
        builder.Services.AddScoped<IAccountService, AccountService>();

        return builder;
    }

    public static WebApplicationBuilder AddIntegrationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ScheduleStatusUpdaterService>();
        builder.Services.AddTransient<IPasswordHasher<Admin>, PasswordHasher<Admin>>();

        return builder;
    }

    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<INewsRepository, NewsRepository>();
        builder.Services.AddScoped<ISocialRepository, SocialRepository>();
        builder.Services.AddScoped<IGroupSocialRepository, GroupSocialRepository>();
        builder.Services.AddScoped<IMemberRepository, MemberRepository>();
        builder.Services.AddScoped<IGroupPageRepository, GroupPageRepository>();
        builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
        builder.Services.AddScoped<IMusicPlatformRepository, MusicPlatformRepository>();
        builder.Services.AddScoped<IVideoRepository, VideoRepository>();
        builder.Services.AddScoped<IAdminRepository, AdminRepository>();
        builder.Services.AddScoped<IBlobRepository, BlobRepository>();

        return builder;
    }

    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(options =>
        {
            options.ISSUER = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("TOKEN_ISSUER");
            options.AUDIENCE = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("TOKEN_AUDIENCE");
            options.KEY = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("TOKEN_KEY");
            options.LIFETIME = int.Parse(EnvironmentExtensions.GetEnvironmentVariableOrThrowException("TOKEN_LIFETIME"));
        });

        builder.Services.AddOptions<CookieSettings>()
            .Bind(builder.Configuration.GetSection("CookieOptions"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    public static WebApplicationBuilder AddDbContext(this WebApplicationBuilder builder)
    {
        string connectionString = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("DB_CLOUD");

        builder.Services.AddDbContext<StoronnimVContext>(options =>
            options.UseNpgsql(connectionString));

        return builder;
    }

    public static WebApplicationBuilder AddFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<LogInRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<EditBasicAdminLoginRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<EditBasicAdminPasswordRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<AddBasicAdminRequestValidator>();

        return builder;
    }

    public static WebApplicationBuilder AddSerilogLogger(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("../logs/log.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
            .CreateLogger();

        builder.Host.UseSerilog();
        builder.Services.AddLogging();

        return builder;
    }

    public static WebApplicationBuilder AddAutoMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(NewsMappingProfile).Assembly);
        builder.Services.AddAutoMapper(typeof(NewsShortMappingProfile).Assembly);

        builder.Services.AddAutoMapper(typeof(ScheduleMappingProfile).Assembly);
        builder.Services.AddAutoMapper(typeof(ScheduleShortMappingProfile).Assembly);

        builder.Services.AddAutoMapper(typeof(GroupPageMappingProfile).Assembly);
        builder.Services.AddAutoMapper(typeof(MemberShortMappingProfile).Assembly);
        builder.Services.AddAutoMapper(typeof(MemberMappingProfile).Assembly);
        builder.Services.AddAutoMapper(typeof(SocialMappingProfile).Assembly);

        builder.Services.AddAutoMapper(typeof(HomeNewsMappingProfile).Assembly);
        builder.Services.AddAutoMapper(typeof(HomeScheduleMappingProfile).Assembly);

        return builder;
    }

    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                var clientUrl = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("CLIENT_URL");
                
                policy.WithOrigins(clientUrl)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddHangfire(this WebApplicationBuilder builder)
    {
        string connectionString = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("DB_CLOUD");

        builder.Services.AddHangfire(config => config
            .UsePostgreSqlStorage(options => { options.UseNpgsqlConnection(connectionString); }));

        builder.Services.AddHangfireServer();

        return builder;
    }

    public static WebApplicationBuilder AddJwtBearer(this WebApplicationBuilder builder)
    {
        var jwtOptions = new JwtOptions
        {
            ISSUER = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("TOKEN_ISSUER"),
            AUDIENCE = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("TOKEN_AUDIENCE"),
            KEY = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("TOKEN_KEY"),
            LIFETIME = int.Parse(EnvironmentExtensions.GetEnvironmentVariableOrThrowException("TOKEN_LIFETIME"))
        };

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.ISSUER,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.AUDIENCE,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtOptions.GetKey()
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string? token = context.Request.Cookies["Token"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("SuperAdminOnly", policy =>
                policy.RequireRole("SuperAdmin"));

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description =
                    "Input your JWT token in the 'Authorization' header like this: \"Authorization: Bearer {yourJWT}\""
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddResponseCompression(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        return builder;
    }

    public static WebApplicationBuilder AddRateLimiter(this WebApplicationBuilder builder)
    {
        var rateLimiterOptions = builder.Configuration.GetSection("RateLimiterOptions")
            .Get<Options.RateLimiter.RateLimiterOptions>();

        if (rateLimiterOptions == null)
        {
            throw new KeyNotFoundException("RateLimiterOptions are not configured correctly.");
        }

        builder.Services.AddRateLimiter(options =>
        {
            rateLimiterOptions.Policies.ForEach(policy =>
            {
                AddLimiterPolicy(options, policy.PolicyName, policy.Limit, policy.Expiration);
            });

            options.RejectionStatusCode = rateLimiterOptions.StatusCode;
        });

        return builder;
    }

    private static void AddLimiterPolicy(RateLimiterOptions options, string policyName, int limit, TimeSpan expiration)
    {
        options.AddPolicy(policyName, _ =>
            RateLimitPartition.GetFixedWindowLimiter(policyName, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = limit,
                Window = expiration
            })
        );
    }

    public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
    {
        string connectionString = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("DB_CLOUD");

        builder.Services.AddHealthChecks()
            .AddCheck("API",
                () => HealthCheckResult.Healthy("API is alive"),
                tags: ["api"])
            .AddNpgSql(connectionString, name: "PostgresSQL", tags: ["database"]);

        return builder;
    }
}