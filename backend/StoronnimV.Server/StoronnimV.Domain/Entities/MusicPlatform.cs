using StoronnimV.Domain.Entities.Shared;

namespace StoronnimV.Domain.Entities;

public class MusicPlatform : BaseEntity
{
    public required string BgImageUrl { get; set; } = string.Empty;
    public required string PlatformUrl { get; set; } = string.Empty;
    
    public MusicPlatform() {}
}