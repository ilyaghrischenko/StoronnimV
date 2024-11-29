using Microsoft.EntityFrameworkCore;

namespace StoronnimV.Data;

public static class DatabaseInitializer
{
    public static void Initialize(StoronnimVContext context)
    {
        try
        {
            context.Database.Migrate();
            Console.WriteLine("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
            throw;
        }
    }
}