using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StoronnimV.Infrastructure;

public sealed class StoronnimVContextFactory : IDesignTimeDbContextFactory<StoronnimVContext>
{
    public StoronnimVContext CreateDbContext(string[] args)
    {
        string? connectionString = Environment.GetEnvironmentVariable("DB_CLOUD");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("DB_CLOUD is required to run Entity Framework migrations.");
        }

        var options = new DbContextOptionsBuilder<StoronnimVContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new StoronnimVContext(options);
    }
}
