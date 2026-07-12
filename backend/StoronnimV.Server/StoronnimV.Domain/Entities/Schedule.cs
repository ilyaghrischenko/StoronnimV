using StoronnimV.Domain.Entities.Shared;
using StoronnimV.Domain.Enums;

namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность, для хранения информации об афишах
/// </summary>
public class Schedule : BaseEntity
{
    public required string Title { get; set; } = string.Empty;
    public required DateTime PerformanceDateTime { get; set; } = DateTime.UtcNow;
    public required string Description { get; set; } = string.Empty;
    public required string Location { get; set; } = string.Empty;
    
    public string? Photo { get; set; }
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Active;
    
    public Schedule() {}
}