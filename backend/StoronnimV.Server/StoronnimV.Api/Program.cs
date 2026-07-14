using AutoMapper;
using DotNetEnv;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using StoronnimV.Api.Extensions;
using StoronnimV.Api.Middlewares;
using StoronnimV.Api.Models;
using StoronnimV.Application.Mapping.Admin;
using StoronnimV.Application.Mapping.Group;
using StoronnimV.Application.Mapping.Home;
using StoronnimV.Application.Mapping.News;
using StoronnimV.Application.Mapping.Schedule;
using StoronnimV.Application.Services.Background;

if (File.Exists(".env"))
{
    var loadOptions = new LoadOptions(clobberExistingVars: false, onlyExactPath: true);
    Env.Load(options: loadOptions);
}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddRepositories()
    .AddApplicationServices()
    .AddIntegrationServices()
    .AddOptions()
    .AddAntiforgeryProtection()
    .AddFluentValidation()
    .AddSerilogLogger()
    .AddAutoMapper()
    .AddCors()
    .AddHangfire()
    .AddDbContext()
    .AddJwtBearer()
    .AddResponseCompression()
    .AddRateLimiter()
    .AddHealthChecks();

#region AutoMapper

MapperConfiguration mapperConfig = new(cfg =>
{
    #region Group

    cfg.AddProfile<GroupPageMappingProfile>();
    cfg.AddProfile<MemberShortMappingProfile>();
    cfg.AddProfile<MemberMappingProfile>();
    cfg.AddProfile<SocialMappingProfile>();

    #endregion

    #region News

    cfg.AddProfile<NewsMappingProfile>();
    cfg.AddProfile<NewsShortMappingProfile>();

    #endregion

    #region Schedule

    cfg.AddProfile<ScheduleMappingProfile>();
    cfg.AddProfile<ScheduleShortMappingProfile>();

    #endregion

    #region Home

    cfg.AddProfile<HomeNewsMappingProfile>();
    cfg.AddProfile<HomeScheduleMappingProfile>();

    #endregion

    #region Admin

    cfg.AddProfile<BasicAdminMappingProfile>();

    #endregion
});

mapperConfig.AssertConfigurationIsValid();

#endregion

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var errors = actionContext.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "The supplied value is invalid."
                            : error.ErrorMessage)
                        .ToArray());
            var response = new ObjectResult(ApiErrorResponse.Create(
                actionContext.HttpContext,
                StatusCodes.Status400BadRequest,
                "One or more validation errors occurred.",
                errors))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            response.ContentTypes.Add("application/problem+json");

            return response;
        };
    });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePages(async statusCodeContext =>
{
    HttpContext context = statusCodeContext.HttpContext;
    int statusCode = context.Response.StatusCode;
    string detail = statusCode switch
    {
        StatusCodes.Status400BadRequest => "The request could not be processed.",
        StatusCodes.Status401Unauthorized => "Authentication is required.",
        StatusCodes.Status403Forbidden => "Access is forbidden.",
        StatusCodes.Status404NotFound => "The requested resource was not found.",
        StatusCodes.Status415UnsupportedMediaType => "The request media type is not supported.",
        _ => "The request failed."
    };

    await context.Response.WriteAsJsonAsync(
        ApiErrorResponse.Create(context, statusCode, detail),
        options: null,
        contentType: "application/problem+json",
        cancellationToken: context.RequestAborted);
});

app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseMiddleware<AntiforgeryMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<LoggingMiddleware>();

app.UseHangfireDashboard();
app.MapHangfireDashboard();

app.UseResponseCompression();
app.UseRateLimiter();
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

#region StatusUpdaterSettings

RecurringJob.AddOrUpdate<ScheduleStatusUpdaterService>(
    "update-schedule-statuses",
    service => service.UpdateScheduleStatusesAsync(CancellationToken.None),
    Cron.Daily);

#endregion

app.MapGet("/", context =>
{
    context.Response.Redirect("/index.html");
    return Task.CompletedTask;
});

app.Run();
