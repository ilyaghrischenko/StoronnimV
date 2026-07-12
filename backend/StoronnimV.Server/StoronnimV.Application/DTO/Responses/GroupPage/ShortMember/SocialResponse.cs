using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.GroupPage.ShortMember;

public class SocialResponse : BaseResponseDto
{
    public string SocialNetwork { get; set; }
    public string Url { get; set; }
    
    public SocialResponse() { }
    
    public SocialResponse(long id, string socialNetwork, string url)
    {
        Id = id;
        SocialNetwork = socialNetwork;
        Url = url;
    }
}