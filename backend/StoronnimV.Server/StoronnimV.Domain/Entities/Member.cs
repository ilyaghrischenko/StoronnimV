using StoronnimV.Domain.Entities.Shared;

namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность, которая представляет участника группы
/// </summary>
public class Member : BaseEntity
{
    public required string PhotoUrl { get; set; } = string.Empty;
    public required string FullName { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required string Role { get; set; } = string.Empty;
    
    public IEnumerable<Social> Socials { get; set; } = new List<Social>();
    
    public Member() {}
}
