using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Application.Contracts.Entities.Shared;

public interface IGetAllService<TProjection> where TProjection : BaseProjection
{
    Task<IEnumerable<TProjection>> GetAllAsync(CancellationToken ct);
}