using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.News;
using StoronnimV.Infrastructure.Repositories.Database.Shared;

namespace StoronnimV.Infrastructure.Repositories.Database;

/// <summary>
/// Репозиторий для конкретной сущности, нужен для описания метода с инклудами, а так же для специальных селект методов
/// </summary>
/// <param name="context"></param>
public class NewsRepository(StoronnimVContext context)
    : Repository<News>(context), INewsRepository
{
    private readonly StoronnimVContext _context = context;

    protected override IQueryable<News> ApplyIncludes(IQueryable<News> dbSet)
    {
        return dbSet.Include(news => news.Video);
    }

    public async Task<NewsFullProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.NewsItems;
        var query = ApplyIncludes(dbSet);

        NewsFullProjection? result = await query
            .AsNoTracking()
            .Select(newsItem => new NewsFullProjection
            {
                Id = newsItem.Id,
                Photo = newsItem.Photo,
                Video = newsItem.Video.Url,
                Title = newsItem.Title,
                Description = newsItem.Description,
                Priority = newsItem.Priority,
                Date = newsItem.Date
            })
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        
        return result;
    }

    public async Task<IEnumerable<NewsPaginationProjection>?> GetForPageAsync(int page, CancellationToken ct, int pageSize, params object[] args)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.NewsItems;
        var query = ApplyIncludes(dbSet);
        
        var result = await query
            .AsNoTracking()
            .OrderByDescending(newsItem => newsItem.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(newsItem => new NewsPaginationProjection
            {
                Id = newsItem.Id,
                Photo = newsItem.Photo,
                Title = newsItem.Title,
                Priority = newsItem.Priority,
                Date = newsItem.Date
            })
            .ToListAsync(ct);
        
        return result;
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct, params object[] args)
    {
        ct.ThrowIfCancellationRequested();

        int result = await _context.NewsItems.CountAsync(ct);
        
        return result;
    }
    
    public async Task<IEnumerable<NewsHomeProjection>?> GetMainNewsForHomePageAsync(int count, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.NewsItems;
        var query = ApplyIncludes(dbSet);
        
        var result = await query
            .AsNoTracking()
            .Where(newsItem => newsItem.Priority == NewsPriority.Main)
            .OrderByDescending(newsItem => newsItem.Date)
            .Take(count)
            .Select(newsItem => new NewsHomeProjection
            {
                Id = newsItem.Id,
                Photo = newsItem.Photo,
                Title = newsItem.Title
            })
            .ToListAsync(ct);
        
        return result;
    }
}