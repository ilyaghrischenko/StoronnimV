using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.News;

public class NewsFullProjection : BaseProjection
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required NewsPriority Priority { get; init; }
    public required DateOnly Date { get; init; }
    public required string? Photo { get; init; }
    public required string? Video { get; init; }
}