using Microsoft.EntityFrameworkCore;
using StoronnimV.Api.Middlewares;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Data;
using StoronnimV.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INewsRepository, NewsRepository>();

builder.Services.AddPooledDbContextFactory<StoronnimVContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionDima")));
    // options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionIlya")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.Run();