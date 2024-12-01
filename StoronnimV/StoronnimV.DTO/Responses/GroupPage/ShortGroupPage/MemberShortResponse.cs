using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.DTO.Responses.GroupPage;

public class MemberShortResponse : BaseDto
{
    public string PhotoUrl { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }

    public MemberShortResponse()
    {
    }

    public MemberShortResponse(long id, string photoUrl, string fullName, string role)
    {
        Id = id;
        PhotoUrl = photoUrl;
        FullName = fullName;
        Role = role;
    }
}