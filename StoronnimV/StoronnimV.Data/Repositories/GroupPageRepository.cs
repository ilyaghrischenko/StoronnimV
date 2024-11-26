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

    public async Task<GroupPage?> GetByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.GroupPages;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<GroupPage>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.GroupPages;
        var query = ApplyIncludes(dbSet);
        
        //TODO: Дописать Селекты
        return await query
            .ToListAsync();
    }
}