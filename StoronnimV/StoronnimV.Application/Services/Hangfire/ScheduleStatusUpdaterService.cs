using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Hangfire;

/// <summary>
/// Сервис для обновления эллементов с таблицы "Афиша", которые активны, но уже вышел срок
/// </summary>
/// <param name="scheduleService"></param>
public class ScheduleStatusUpdaterService(IScheduleService scheduleService)
{
    private readonly IScheduleService _scheduleService = scheduleService;

    public async Task UpdateScheduleStatusesAsync()
    {
        await _scheduleService.UpdateStatusesAsync();
    }
}