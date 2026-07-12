using StoronnimV.Application.Models;
using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Application.Contracts.Entities.Shared;

public interface IPaginationService<TProjection> where TProjection : BaseProjection
{
    Task<PaginationResult<TProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct, params object[] args);
}