using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.DTO.Responses.GroupPage.ShortMember;

public class MemberResponse : BaseDto
{
    public string PhotoUrl { get; set; }
    public string FullName { get; set; }
    public string Description { get; set; }
    public string Role { get; set; }

    public MemberResponse(){}

    public MemberResponse(long id, string photoUrl, string fullName, string description, string role)
    {
        Id = id;
        PhotoUrl = photoUrl;
        FullName = fullName;
        Description = description;
        Role = role;
    }
}