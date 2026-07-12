using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Contracts.Database.Shared;

public interface IPaginationRepository<TProjection> where TProjection : BaseProjection
{
    Task<IEnumerable<TProjection>?> GetForPageAsync(int page, CancellationToken ct, int pageSize = 10, params object[] args);
    Task<int> GetTotalCountAsync(CancellationToken ct, params object[] args);
}