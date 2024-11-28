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

    public async Task<object?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.NewsItems;
        var query = ApplyIncludes(dbSet);

        return await query
            .AsNoTracking()
            .Select(newsItem => new
            {
                Id = newsItem.Id,
                Photo = newsItem.Photo,
                Title = newsItem.Title,
                Description = newsItem.Description,
                Priority = newsItem.Priority.ToString(),
                Date = newsItem.Date.ToShortDateString()
            })
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<object?> GetByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.NewsItems;
        var query = ApplyIncludes(dbSet);

        return await query
            .Select(newsItem => new
            {
                Id = newsItem.Id,
                Photo = newsItem.Photo,
                Title = newsItem.Title,
                Description = newsItem.Description,
                Priority = newsItem.Priority.ToString(),
                Date = newsItem.Date.ToShortDateString()
            })
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<object>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.NewsItems;
        var query = ApplyIncludes(dbSet);
        
        return await query
            .AsNoTracking()
            .Select(newsItem => new
            {
                Id = newsItem.Id,
                Photo = newsItem.Photo,
                Title = newsItem.Title,
                Description = newsItem.Description,
                Priority = newsItem.Priority.ToString(),
                Date = newsItem.Date.ToShortDateString()
            })
            .ToListAsync();
    }
}