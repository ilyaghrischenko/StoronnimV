using StoronnimV.Domain.Entities.Shared;

namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность, которая представляет участника группы
/// </summary>
public class Member : BaseEntity
{
    public string PhotoUrl { get; set; }
    public string FullName { get; set; }
    public string Description { get; set; }
    public string Role { get; set; }
    
    public virtual IEnumerable<Social> Socials { get; set; } = new List<Social>();
    
    public Member() {}
    public Member(string photoUrl, string fullName, string description, string role)
    {
        PhotoUrl = photoUrl;
        FullName = fullName;
        Description = description;
        Role = role;
    }
    
}
