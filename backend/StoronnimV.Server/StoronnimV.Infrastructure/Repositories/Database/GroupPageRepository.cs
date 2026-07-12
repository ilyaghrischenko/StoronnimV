using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections;
using StoronnimV.Infrastructure.Repositories.Database.Shared;

namespace StoronnimV.Infrastructure.Repositories.Database;

/// <summary>
/// Репозиторий для получения данных напрямую с бд
/// </summary>
/// <param name="context"></param>
public class GroupPageRepository(StoronnimVContext context) : 
    Repository<GroupPage>(context), IGroupPageRepository
{
    private readonly StoronnimVContext _context = context;

    public async Task<GroupPageProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.GroupPages;
        var query = ApplyIncludes(dbSet);

        GroupPageProjection? result = await query
            .AsNoTracking()
            .Select(groupPage => new GroupPageProjection
            {
                Id = groupPage.Id,
                PhotoUrl = groupPage.PhotoUrl,
                Description = groupPage.Description
            })
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        
        return result;
    }

    public async Task<IEnumerable<GroupPageProjection>?> GetAllAsNoTrackingAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.GroupPages;
        var query = ApplyIncludes(dbSet);
        
        var result = await query
            .AsNoTracking()
            .Select(groupPage => new GroupPageProjection
            {
                Id = groupPage.Id,
                PhotoUrl = groupPage.PhotoUrl,
                Description = groupPage.Description
            })
            .ToListAsync(ct);
        
        return result;
    }
    
    public async Task<GroupPageProjection?> GetFirstGroupPageAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.GroupPages;
        var query = ApplyIncludes(dbSet);
        
        GroupPageProjection? result = await query
            .AsNoTracking()
            .Select(groupPage => new GroupPageProjection
            {
                Id = groupPage.Id,
                PhotoUrl = groupPage.PhotoUrl,
                Description = groupPage.Description
            })
            .FirstOrDefaultAsync(ct);
        
        return result;
    }
}