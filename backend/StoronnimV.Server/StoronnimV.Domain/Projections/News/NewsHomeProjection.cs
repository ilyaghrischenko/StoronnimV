using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.News;

public class NewsHomeProjection : BaseProjection
{
    public required string Title { get; init; }
    public required string? Photo { get; init; }
}