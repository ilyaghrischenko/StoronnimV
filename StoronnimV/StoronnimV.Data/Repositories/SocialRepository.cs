using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data.Repositories;

public class SocialRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<Social>(contextFactory), ISocialRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;

    public IQueryable<Social> ApplyIncludes(IQueryable<Social> dbSet)
    {
        return dbSet;
    }

    public async Task<Social?> GetByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Socials;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Social>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Socials;
        var query = ApplyIncludes(dbSet);
        
        //TODO: Дописать Селекты
        return await query
            .ToListAsync();
    }
}