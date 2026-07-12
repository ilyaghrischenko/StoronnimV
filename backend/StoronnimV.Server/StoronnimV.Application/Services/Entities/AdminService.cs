using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.Exceptions;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Projections.Admin;

namespace StoronnimV.Application.Services.Entities;

public class AdminService(
    IAdminRepository adminRepository) : IAdminService
{
    public async Task<AdminProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        AdminProjection? admin = await adminRepository.GetByIdAsNoTrackingAsync(id, ct);

        if (admin is null)
        {
            throw new EntityNotFoundException($"Admin with {nameof(id)}: {id} was not found");
        }

        return admin;
    }
}