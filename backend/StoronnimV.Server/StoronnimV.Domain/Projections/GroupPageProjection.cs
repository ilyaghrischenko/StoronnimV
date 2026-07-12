using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections;

public class GroupPageProjection : BaseProjection
{
    public required string PhotoUrl { get; init; }
    public required string Description { get; init; }
}