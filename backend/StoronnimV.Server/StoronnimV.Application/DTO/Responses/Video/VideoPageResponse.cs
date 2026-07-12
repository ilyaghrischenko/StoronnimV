using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.Video;

public class VideoPageResponse : BaseResponseDto
{
    public required string Title { get; init; }
    public required string Url { get; init; }
    public required string Type { get; init; }
}