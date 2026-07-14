using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

/// <summary>
/// DTO для запроса добавления афиша
/// </summary>
public class ScheduleAdditionRequest
{
    public required string Title { get; init; }
    [BindRequired]
    public required DateTime PerformanceDateTime { get; init; }
    public required string Description { get; init; }
    public required string Location { get; init; }
    public IFormFile? Photo { get; init; }
    public required string Status { get; init; }
}
