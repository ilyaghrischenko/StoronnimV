using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Admin;
using StoronnimV.Infrastructure.Repositories.Database.Shared;

namespace StoronnimV.Infrastructure.Repositories.Database;

public class AdminRepository(
    StoronnimVContext context)
    : Repository<Admin>(context), IAdminRepository
{
    private readonly StoronnimVContext _context = context;

    public async Task<AdminProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var dbSet = _context.Admins;
        var query = ApplyIncludes(dbSet);

        AdminProjection? result = await query
            .AsNoTracking()
            .Select(admin => new AdminProjection
            {
                Id = admin.Id,
                Login = admin.Login,
                Password = admin.Password
            })
            .FirstOrDefaultAsync(admin => admin.Id == id, ct);

        return result;
    }

    public async Task<Admin?> GetByLoginAsync(string login, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.Admins;
        var query = ApplyIncludes(dbSet);

        Admin? result = await query
            .AsNoTracking()
            .FirstOrDefaultAsync(admin => admin.Login == login, ct);

        return result;
    }

    public async Task<IEnumerable<BasicAdminProjection>?> GetAllBasicAdminsAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.Admins;
        var query = ApplyIncludes(dbSet);

        var result = await query
            .AsNoTracking()
            .Where(admin => admin.Type == AdminType.Basic)
            .Select(admin => new BasicAdminProjection
            {
                Id = admin.Id,
                Login = admin.Login
            })
            .ToListAsync(ct);
        
        return result;
    }
}