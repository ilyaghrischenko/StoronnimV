using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.HomePage;

public class ScheduleHomeResponse : BaseResponseDto
{
    public required string Photo { get; init; }
    public required string Title { get; init; }
    public required string PerformanceDateTime { get; init; }
    public required string Location { get; init; }
}