using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Hangfire;

public class ScheduleStatusUpdaterService(IScheduleService scheduleService)
{
    private readonly IScheduleService _scheduleService = scheduleService;

    public async Task UpdateScheduleStatusesAsync()
    {
        await _scheduleService.UpdateStatusesAsync();
    }
}