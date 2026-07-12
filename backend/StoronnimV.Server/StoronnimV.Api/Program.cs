using AutoMapper;
using DotNetEnv;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using StoronnimV.Api.Extensions;
using StoronnimV.Api.Middlewares;
using StoronnimV.Application.Mapping.Admin;
using StoronnimV.Application.Mapping.Group;
using StoronnimV.Application.Mapping.Home;
using StoronnimV.Application.Mapping.News;
using StoronnimV.Application.Mapping.Schedule;
using StoronnimV.Application.Services.Background;

if (File.Exists(".env"))
{
    var loadOptions = new LoadOptions(onlyExactPath: true);
    Env.Load(options: loadOptions);
}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddRepositories()
    .AddApplicationServices()
    .AddIntegrationServices()
    .AddOptions()
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
builder.Services.AddControllers();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ExceptionMiddleware>();
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