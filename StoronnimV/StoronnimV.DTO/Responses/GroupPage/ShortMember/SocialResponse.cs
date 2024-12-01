using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.DTO.Responses.GroupPage;

public class SocialResponse : BaseDto
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