using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace StoronnimV.Data;

/// <summary>
/// Класс, который используется для создания объекта контекста для разработки.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StoronnimVContext>
{
    public StoronnimVContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var relativePath = "../StoronnimV.Api";
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(currentDirectory, relativePath))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("LocalConnectionDima");

        var optionsBuilder = new DbContextOptionsBuilder<StoronnimVContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new StoronnimVContext(optionsBuilder.Options);
    }
}