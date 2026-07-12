using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.Schedule;

public class ScheduleShortProjection : BaseProjection
{
    public required string Title { get; init; }
    public required DateTime PerformanceDateTime { get; init; }
    public required string Location { get; init; }
    
    public required string? Photo { get; init; }
}