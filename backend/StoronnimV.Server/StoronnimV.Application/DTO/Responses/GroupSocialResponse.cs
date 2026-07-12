using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses;

public class GroupSocialResponse : BaseResponseDto
{
    public required string PhotoUrl { get; init; }
    public required string Name { get; init; }
    public required string LinkUrl { get; init; }
}