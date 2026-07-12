using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.Video;

public class VideoFullProjection : BaseProjection
{
    public required string Title { get; init; }
    public required string Url { get; init; }
    public required VideoType Type { get; init; }
}