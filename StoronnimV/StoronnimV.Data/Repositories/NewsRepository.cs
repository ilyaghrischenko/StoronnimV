using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data.Repositories;

public class NewsRepository(IDbContextFactory<StoronnimVContext> contextFactory)
    : Repository<News>(contextFactory), INewsRepository
{
    protected override IQueryable<News> ApplyIncludes(IQueryable<News> dbSet)
    {
        return base.ApplyIncludes(dbSet);
    }
    
    
}