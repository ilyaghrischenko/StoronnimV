using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections.Admin;

namespace StoronnimV.Domain.Contracts.Database;

public interface IAdminRepository
    : IRepository<Admin>, IGetByIdRepository<AdminProjection>
{
    Task<Admin?> GetByLoginAsync(string login, CancellationToken ct);
    Task<IEnumerable<BasicAdminProjection>?> GetAllBasicAdminsAsync(CancellationToken ct);
}