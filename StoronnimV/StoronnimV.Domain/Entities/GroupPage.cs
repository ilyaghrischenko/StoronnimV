namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность для страницы о группе
/// </summary>
public class GroupPage : BaseEntity
{
    public string PhotoUrl { get; set; }
    public string Description { get; set; }
    
    public GroupPage() {}
    public GroupPage(string photoUrl, string description)
    {
        PhotoUrl = photoUrl;
        Description = description;
    }
}