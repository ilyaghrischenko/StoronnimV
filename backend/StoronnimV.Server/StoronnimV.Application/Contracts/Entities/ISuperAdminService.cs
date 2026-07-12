using StoronnimV.Application.Contracts.Entities.Shared;
using StoronnimV.Domain.Projections.Admin;

namespace StoronnimV.Application.Contracts.Entities;

public interface ISuperAdminService : IGetAllService<BasicAdminProjection>
{
    Task DeleteBasicAdminAsync(long id, CancellationToken ct);
    Task<BasicAdminProjection> AddBasicAdminAsync(string login, string unhashedPassword, CancellationToken ct);
    Task<BasicAdminProjection> EditBasicAdminLoginAsync(long id, string newlogin, CancellationToken ct);
    Task EditBasicAdminPasswordAsync(long id, string oldPassword, string newUnhashedPassword, CancellationToken ct);
}