using StoronnimV.Application.Contracts.Entities;

namespace StoronnimV.Application.Services.Background;

/// <summary>
/// Сервис для обновления эллементов с таблицы "Афиша", которые активны, но уже вышел срок
/// </summary>
/// <param name="scheduleService"></param>
public class ScheduleStatusUpdaterService(IScheduleService scheduleService)
{
    public async Task UpdateScheduleStatusesAsync(CancellationToken ct)
    {
        await scheduleService.UpdateStatusesAsync(ct);
    }
}