using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Contracts.Database.Shared;

public interface IGetByIdRepository<TProjection> where TProjection : BaseProjection
{
    Task<TProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct);
}