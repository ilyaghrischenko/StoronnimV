using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Data.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Data.Repositories;

/// <summary>
/// Репозиторий для получения данных напрямую с бд
/// </summary>
/// <param name="contextFactory"></param>
public class SocialRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<Social>(contextFactory), ISocialRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;

    protected override IQueryable<Social> ApplyIncludes(IQueryable<Social> dbSet)
    {
        return dbSet.Include(social => social.Member);
    }

    public async Task<object?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Socials;
        var query = ApplyIncludes(dbSet);

        return await query
            .AsNoTracking()
            .Select(social => new
            {
                Id = social.Id,
                Type = social.Type.ToString(),
                Url = social.Url
            })
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<object>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Socials;
        var query = ApplyIncludes(dbSet);
        
        return await query
            .AsNoTracking()
            .Select(social => new
            {
                Id = social.Id,
                Name = social.Member.FullName,
                Type = social.Type.ToString(),
                Url = social.Url
            })
            .ToListAsync();
    }
    
    public async Task<IEnumerable<object>?> GetAllForMemberAsync(long memberId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Socials;
        var query = ApplyIncludes(dbSet);
        
        return await query
            .AsNoTracking()
            .Where(social => social.Member.Id == memberId)
            .Select(social => new
            {
                Id = social.Id,
                Type = social.Type.ToString(),
                Url = social.Url
            })
            .ToListAsync();
    }

}