using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections;
using StoronnimV.Infrastructure.Repositories.Database.Shared;

namespace StoronnimV.Infrastructure.Repositories.Database;

public class GroupSocialRepository(StoronnimVContext context)
    : Repository<GroupSocial>(context), IGroupSocialRepository
{
    private readonly StoronnimVContext _context = context;

    public async Task<GroupSocialProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        GroupSocialProjection? result = await _context.GroupSocials
            .AsNoTracking()
            .Select(groupSocial => new GroupSocialProjection
            {
                Id = groupSocial.Id,
                PhotoUrl = groupSocial.PhotoUrl,
                Name = groupSocial.Name,
                LinkUrl = groupSocial.LinkUrl
            })
            .FirstOrDefaultAsync(groupSocial => groupSocial.Id == id, ct);
        
        return result;
    }

    public async Task<IEnumerable<GroupSocialProjection>?> GetAllAsNoTrackingAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var result = await _context.GroupSocials
            .AsNoTracking()
            .Select(groupSocial => new GroupSocialProjection
            {
                Id = groupSocial.Id,
                PhotoUrl = groupSocial.PhotoUrl,
                Name = groupSocial.Name,
                LinkUrl = groupSocial.LinkUrl
            })
            .ToListAsync(ct);
        
        return result;
    }
}