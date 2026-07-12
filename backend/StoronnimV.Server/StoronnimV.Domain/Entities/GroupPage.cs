using StoronnimV.Domain.Entities.Shared;

namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность для страницы о группе
/// </summary>
public class GroupPage : BaseEntity
{
    public required string PhotoUrl { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    
    public GroupPage() {}
}