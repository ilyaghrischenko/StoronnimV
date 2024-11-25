using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data;

/// <summary>
/// Класс, который нужен для описания бд, взаимойдействий между таблицами и тд
/// </summary>
public class StoronnimVContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public StoronnimVContext(DbContextOptions<StoronnimVContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        var connectionString = _configuration.GetConnectionString("LocalConnection");
        optionsBuilder.UseNpgsql(connectionString);
    }
    
    public virtual DbSet<News> NewsItems { get; set; }
}