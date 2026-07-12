using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.Schedule;

public class ScheduleFullProjection : BaseProjection
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DateTime PerformanceDateTime { get; init; }
    public required string Location { get; init; }
    public required ScheduleStatus Status { get; init; }
    
    public required string? Photo { get; init; }
}