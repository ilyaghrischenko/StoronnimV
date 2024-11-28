using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data.Repositories;

public class GroupPageRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<GroupPage>(contextFactory), IGroupPageRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;

    public IQueryable<GroupPage> ApplyIncludes(IQueryable<GroupPage> dbSet)
    {
        return dbSet;
    }

    public async Task<object?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.GroupPages;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<object?> GetByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.GroupPages;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<object>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.GroupPages;
        var query = ApplyIncludes(dbSet);
        
        //TODO: Дописать Селекты
        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}