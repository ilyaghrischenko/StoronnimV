using Microsoft.EntityFrameworkCore;

namespace StoronnimV.Data;

public static class DatabaseInitializer
{
    public static void Initialize(StoronnimVContext context)
    {
        if (context.Database.EnsureCreated())
        {
            context.Database.Migrate();
        }
        else
        {
            Console.WriteLine("Database already exists.");
        }
    }
}