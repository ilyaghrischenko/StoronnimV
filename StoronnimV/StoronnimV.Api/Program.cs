using Microsoft.EntityFrameworkCore;
using Serilog;
using StoronnimV.Api.Middlewares;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Data;
using StoronnimV.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("../logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddLogging();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ISocialRepository, SocialRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IGroupPageRepository, GroupPageRepository>();

builder.Services.AddPooledDbContextFactory<StoronnimVContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionDima")));
    // options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionIlya")));

var app = builder.Build();

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