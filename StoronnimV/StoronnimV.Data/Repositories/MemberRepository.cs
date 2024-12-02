using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Data.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Data.Repositories;

/// <summary>
/// Репозиторий для получения данных напрямую с бд
/// </summary>
/// <param name="contextFactory"></param>
public class MemberRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<Member>(contextFactory), IMemberRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;
    
    public async Task<object?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Members;
        var query = ApplyIncludes(dbSet);

        return await query
            .AsNoTracking()
            .Select(member => new
            {
                Id = member.Id,
                PhotoUrl = member.PhotoUrl,
                FullName = member.FullName,
                Description = member.Description,
                Role = member.Role
            })
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<object>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Members;
        var query = ApplyIncludes(dbSet);
        
        return await query
            .AsNoTracking()
            .Select(member => new
            {
                Id = member.Id,
                PhotoUrl = member.PhotoUrl,
                FullName = member.FullName,
                Role = member.Role
            })
            .ToListAsync();
    }
}