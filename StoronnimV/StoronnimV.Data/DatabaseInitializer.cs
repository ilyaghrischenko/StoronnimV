using Microsoft.EntityFrameworkCore;

namespace StoronnimV.Data;

/// <summary>
/// Статический класс для проверки создана ли бд при старте программы? Если нет, создаст и применит миграции, если да ничего не произойдёт
/// </summary>
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