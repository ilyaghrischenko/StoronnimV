using StoronnimV.Domain.Enums;

namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность, для хранения информации о новости
/// </summary>
public class News : BaseEntity
{
    public string Photo { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public NewsPriority Priority { get; set; }
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    public News() {}
    public News(string photo, string title, string description, NewsPriority priority)
    {
        Photo = photo;
        Title = title;
        Description = description;
        Priority = priority;
    }
}