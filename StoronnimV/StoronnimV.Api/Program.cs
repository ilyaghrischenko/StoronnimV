using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StoronnimV.Api.Middlewares;
using StoronnimV.Application.Mapping;
using StoronnimV.Application.Services.Controllers;
using StoronnimV.Application.Services.Entities;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.Data;
using StoronnimV.Data.Repositories;

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
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
mapperConfig.AssertConfigurationIsValid();
#endregion

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

#region Dependency Injection
#region Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ISocialRepository, SocialRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IGroupPageRepository, GroupPageRepository>();
#endregion

#region Services
#region Entities
builder.Services.AddScoped<INewsService, NewsService>();
#endregion

#region Controllers
builder.Services.AddScoped<INewsControllerService, NewsControllerService>();
#endregion
#endregion
#endregion

builder.Services.AddPooledDbContextFactory<StoronnimVContext>(options =>
    // options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionDima")));
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionIlya")));

var app = builder.Build();

#region DatabaseInitializer
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<StoronnimVContext>();
        DatabaseInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
    }
}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<ExceptionMiddleware>();

app.Run();