namespace StoronnimV.Domain.Entities.Shared;

/// <summary>
/// Базовая сущность, хранит общие свойства для всех остальных: Id, CreatedAt, UpdatedAt
/// </summary>
public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}