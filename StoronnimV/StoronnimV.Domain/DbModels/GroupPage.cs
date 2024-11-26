namespace StoronnimV.Domain.DbModels;

public class GroupPage : BaseEntity
{
    public string PhotoUrl { get; set; }
    public string Description { get; set; }
    
    public GroupPage(string photoUrl, string description)
    {
        PhotoUrl = photoUrl;
        Description = description;
    }
}