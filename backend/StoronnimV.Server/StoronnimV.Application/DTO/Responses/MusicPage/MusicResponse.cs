using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.MusicPage;

public class MusicResponse : BaseResponseDto
{
    public required string BgImageUrl { get; init; }
    public required string PlatformUrl { get; init; }
}