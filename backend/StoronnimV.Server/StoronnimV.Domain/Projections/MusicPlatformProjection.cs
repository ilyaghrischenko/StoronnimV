using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections;

public class MusicPlatformProjection : BaseProjection
{
    public required string BgImageUrl { get; init; }
    public required string PlatformUrl { get; init; }
}