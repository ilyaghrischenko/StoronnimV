using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.GroupPage.ShortGroupPage;

public class GroupPageResponse : BaseResponseDto
{
    public required string PhotoUrl { get; init; }
    public required string Description { get; init; }
}