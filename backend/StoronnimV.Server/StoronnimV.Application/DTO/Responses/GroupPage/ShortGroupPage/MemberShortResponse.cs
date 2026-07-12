using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.GroupPage.ShortGroupPage;

public class MemberShortResponse : BaseResponseDto
{
    public required string PhotoUrl { get; init; }
    public required string FullName { get; init; }
    public required string Role { get; init; }
}