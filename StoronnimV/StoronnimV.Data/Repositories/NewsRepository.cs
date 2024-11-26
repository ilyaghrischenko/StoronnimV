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
    protected override IQueryable<News> ApplyIncludes(IQueryable<News> dbSet)
    {
        return base.ApplyIncludes(dbSet);
    }
    
    
}