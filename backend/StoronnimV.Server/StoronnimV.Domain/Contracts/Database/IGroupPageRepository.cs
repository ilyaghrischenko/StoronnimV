using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Domain.Contracts.Database;

public interface IGroupPageRepository
    : IRepository<GroupPage>, IGetByIdRepository<GroupPageProjection>, IGetAllRepository<GroupPageProjection>
{
    public Task<GroupPageProjection?> GetFirstGroupPageAsync(CancellationToken ct);
}