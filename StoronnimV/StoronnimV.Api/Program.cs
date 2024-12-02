using AutoMapper;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StoronnimV.Api.Middlewares;
using StoronnimV.Application.Mapping.Group;
using StoronnimV.Application.Mapping.News;
using StoronnimV.Application.Mapping.Schedule;
using StoronnimV.Application.Services.Controllers;
using StoronnimV.Application.Services.Entities;
using StoronnimV.Application.Services.Hangfire;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.Data;
using StoronnimV.Data.Repositories;
using StoronnimV.Data.Repositories.Shared;

var builder = WebApplication.CreateBuilder(args);

#region Logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("../logs/log.json",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddLogging();
#endregion

#region AutoMapper
#region News
builder.Services.AddAutoMapper(typeof(NewsMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(NewsShortMappingProfile).Assembly);
#endregion

#region Schedule
builder.Services.AddAutoMapper(typeof(ScheduleMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ScheduleShortMappingProfile).Assembly);
#endregion

#region Group
builder.Services.AddAutoMapper(typeof(GroupPageMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MemberShortMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MemberMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(SocialMappingProfile).Assembly);
#endregion

var mapperConfig = new MapperConfiguration(cfg =>
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
    
});

mapperConfig.AssertConfigurationIsValid();
#endregion

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

#region Dependency Injection
#region Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ISocialRepository, SocialRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IGroupPageRepository, GroupPageRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
#endregion

#region Services
#region Entities
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ISocialService, SocialService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IGroupPageService, GroupPageService>();
#endregion

#region Controllers
builder.Services.AddScoped<INewsControllerService, NewsControllerService>();
builder.Services.AddScoped<ISchedulesControllerService, SchedulesControllerService>();
builder.Services.AddScoped<IGroupPageControllerService, GroupPageControllerService>();
#endregion

#region Hangfire
builder.Services.AddScoped<ScheduleStatusUpdaterService>();
#endregion
#endregion
#endregion

// var _connectionString = builder.Configuration.GetConnectionString("LocalConnectionDima");
// var _connectionString = builder.Configuration.GetConnectionString("LocalConnectionZhenya");
var connectionString = builder.Configuration.GetConnectionString("LocalConnectionIlya");

#region Hangfire
builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(connectionString));

builder.Services.AddHangfireServer();
#endregion

builder.Services.AddPooledDbContextFactory<StoronnimVContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHangfireDashboard();
app.MapHangfireDashboard();

#region DatabaseInitializer
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContextFactory = services.GetRequiredService<IDbContextFactory<StoronnimVContext>>();
        using var context = dbContextFactory.CreateDbContext();
        DatabaseInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
    }
}
#endregion

#region StatusUpdaterSettings
RecurringJob.AddOrUpdate<ScheduleStatusUpdaterService>(
    "update-schedule-statuses",
    service => service.UpdateScheduleStatusesAsync(),
    Cron.Daily);
#endregion

app.Run();