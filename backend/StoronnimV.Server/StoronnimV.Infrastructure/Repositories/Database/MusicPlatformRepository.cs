using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections;
using StoronnimV.Infrastructure.Repositories.Database.Shared;

namespace StoronnimV.Infrastructure.Repositories.Database;

public class MusicPlatformRepository(StoronnimVContext context)
    : Repository<MusicPlatform>(context), IMusicPlatformRepository
{
    private readonly StoronnimVContext _context = context;

    public async Task<MusicPlatformProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.MusicPlatforms;
        var query = ApplyIncludes(dbSet);
        
        MusicPlatformProjection? result = await query
            .AsNoTracking()
            .Select(musicPlatform => new MusicPlatformProjection
            {
                Id = musicPlatform.Id,
                BgImageUrl = musicPlatform.BgImageUrl,
                PlatformUrl = musicPlatform.PlatformUrl
            })
            .FirstOrDefaultAsync(musicPlatform => musicPlatform.Id == id, ct);
        
        return result;
    }

    public async Task<IEnumerable<MusicPlatformProjection>?> GetAllAsNoTrackingAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.MusicPlatforms;
        var query = ApplyIncludes(dbSet);
        
        var result = await query
            .AsNoTracking()
            .Select(musicPlatform => new MusicPlatformProjection
            {
                Id = musicPlatform.Id,
                BgImageUrl = musicPlatform.BgImageUrl,
                PlatformUrl = musicPlatform.PlatformUrl
            })
            .ToListAsync(ct);
        
        return result;
    }
}