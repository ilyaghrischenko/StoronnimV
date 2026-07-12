using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.SchedulePage;

public class ScheduleShortResponse : BaseResponseDto
{
    public required string Title  { get; init; }
    public required string PerformanceDateTime  { get; init; }
    public required string Location { get; init; }
    
    public string? Photo { get; init; } = string.Empty;
}