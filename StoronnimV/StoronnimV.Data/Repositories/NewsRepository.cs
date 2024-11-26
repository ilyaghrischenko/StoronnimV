using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data.Repositories;

/// <summary>
/// Репозиторий для конкретной сущности, нужен для описания метода с инклудами, а так же для специальных селект методов
/// </summary>
/// <param name="contextFactory"></param>
public class NewsRepository(IDbContextFactory<StoronnimVContext> contextFactory)
    : Repository<News>(contextFactory), INewsRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;
    
    public IQueryable<News> ApplyIncludes(IQueryable<News> dbSet)
    {
        return dbSet;
    }

    public async Task<News?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.NewsItems;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<News?> GetByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.NewsItems;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<News>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.NewsItems;
        var query = ApplyIncludes(dbSet);
        
        //TODO: Дописать Селекты
        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}