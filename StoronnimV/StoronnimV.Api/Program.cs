using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Data;
using StoronnimV.Data.Repositories;
using StoronnimV.Domain.DbModels;
using StoronnimV.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INewsRepository, NewsRepository>();

builder.Services.AddDbContextFactory<StoronnimVContext>(options =>
    // options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionDima")));
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionIlya")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();