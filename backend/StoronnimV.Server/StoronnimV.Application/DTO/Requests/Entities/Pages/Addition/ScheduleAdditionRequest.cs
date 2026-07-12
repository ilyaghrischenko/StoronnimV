using Microsoft.AspNetCore.Http;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

/// <summary>
/// DTO для запроса добавления афиша
/// </summary>
public class ScheduleAdditionRequest
{
    public required string Title { get; init; }
    public required string PerformanceDateTime { get; init; }
    public required string Description { get; init; }
    public required string Location { get; init; }
    public IFormFile? Photo { get; init; }
    public required string Status { get; init; }
}