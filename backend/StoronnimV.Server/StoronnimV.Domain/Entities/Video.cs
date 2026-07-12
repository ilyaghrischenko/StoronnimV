using StoronnimV.Domain.Entities.Shared;
using StoronnimV.Domain.Enums;

namespace StoronnimV.Domain.Entities;

public class Video : BaseEntity
{
    public required string Url { get; set; } = string.Empty;
    public required string BlobName { get; set; } = string.Empty;
    public required string Title { get; set; } = string.Empty;
    public required VideoType Type { get; set; } = VideoType.Performance;
    
    public Video() {}
}