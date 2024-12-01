using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.DTO.Responses.GroupPage.ShortGroupPage;

public class GroupPageResponse : BaseDto
{
    public string PhotoUrl { get; set; }
    public string Description { get; set; }
    
    public GroupPageResponse() { }
    
    public GroupPageResponse(long id, string photoUrl, string description)
    {
        Id = id;
        PhotoUrl = photoUrl;
        Description = description;
    }
}