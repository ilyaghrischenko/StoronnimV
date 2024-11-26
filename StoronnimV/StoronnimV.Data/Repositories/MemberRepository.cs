using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data.Repositories;

public class MemberRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<Member>(contextFactory), IMemberRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;
    
    public IQueryable<Member> ApplyIncludes(IQueryable<Member> dbSet)
    {
        return dbSet;
    }

    public async Task<Member?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Members;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Member?> GetByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Members;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Member>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Members;
        var query = ApplyIncludes(dbSet);
        
        //TODO: Дописать Селекты
        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}