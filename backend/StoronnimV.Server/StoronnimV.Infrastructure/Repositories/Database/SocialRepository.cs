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
public class SocialRepository(StoronnimVContext context)
    : Repository<Social>(context), ISocialRepository
{
    private readonly StoronnimVContext _context = context;

    protected override IQueryable<Social> ApplyIncludes(IQueryable<Social> dbSet)
    {
        return dbSet.Include(social => social.Member);
    }

    public async Task<SocialProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.Socials;
        var query = ApplyIncludes(dbSet);

        SocialProjection? result = await query
            .AsNoTracking()
            .Select(social => new SocialProjection
            {
                Id = social.Id,
                Type = social.Type,
                Url = social.Url
            })
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        
        return result;
    }
}