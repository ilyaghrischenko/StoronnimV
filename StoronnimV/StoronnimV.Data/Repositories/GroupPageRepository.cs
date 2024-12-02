using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Data.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Data.Repositories;

/// <summary>
/// Репозиторий для получения данных напрямую с бд
/// </summary>
/// <param name="contextFactory"></param>
public class GroupPageRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<GroupPage>(contextFactory), IGroupPageRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;

    public async Task<object?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.GroupPages;
        var query = ApplyIncludes(dbSet);

        return await query
            .AsNoTracking()
            .Select(groupPage => new
            {
                Id = groupPage.Id,
                PhotoUrl = groupPage.PhotoUrl,
                Description = groupPage.Description
            })
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<object>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.GroupPages;
        var query = ApplyIncludes(dbSet);
        
        return await query
            .AsNoTracking()
            .Select(groupPage => new
            {
                Id = groupPage.Id,
                PhotoUrl = groupPage.PhotoUrl,
                Description = groupPage.Description
            })
            .ToListAsync();
    }
    
    public async Task<object?> GetFirstGroupPageAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.GroupPages;
        var query = ApplyIncludes(dbSet);
        
        return await query
            .AsNoTracking()
            .Select(groupPage => new
            {
                Id = groupPage.Id,
                PhotoUrl = groupPage.PhotoUrl,
                Description = groupPage.Description
            })
            .FirstOrDefaultAsync();
    }
}