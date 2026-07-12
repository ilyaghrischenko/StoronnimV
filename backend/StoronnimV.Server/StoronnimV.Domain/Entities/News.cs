using StoronnimV.Domain.Entities.Shared;
using StoronnimV.Domain.Enums;

namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность, для хранения информации о новости
/// </summary>
public class News : BaseEntity
{
    public required string Title { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public string? Photo { get; set; } = null;
    public Video? Video { get; set; } = null;
    public NewsPriority Priority { get; set; } = NewsPriority.Secondary;
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    public News() {}
}