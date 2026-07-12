using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.GroupPage.ShortMember;

public class MemberResponse : BaseResponseDto
{
    public required string PhotoUrl { get; init; }
    public required string FullName { get; init; }
    public required string Description { get; init; }
    public required string Role { get; init; }
}