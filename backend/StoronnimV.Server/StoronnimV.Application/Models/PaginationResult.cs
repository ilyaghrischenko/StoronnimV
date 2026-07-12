using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Application.Models;

public class PaginationResult<TProjection> where TProjection : BaseProjection
{
    public required int CurrentPage { get; init; }
    public required int TotalPages { get; init; }
    public required int TotalItems { get; init; }
    public required IEnumerable<TProjection> Items { get; init; }
}