using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Data.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Data.Repositories;

public class SocialRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<Social>(contextFactory), ISocialRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;

    public async Task<object?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Socials;
        var query = ApplyIncludes(dbSet);

        //TODO: Дописать Селекты
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<object>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Socials;
        var query = ApplyIncludes(dbSet);
        
        //TODO: Дописать Селекты
        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}