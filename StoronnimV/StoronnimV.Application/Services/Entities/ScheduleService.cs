using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Extensions;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

public class ScheduleService(IScheduleRepository scheduleRepository) : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository = scheduleRepository;
    
    public async Task<object> GetItemByIdAsync(long id)
    {
        var schedule = await _scheduleRepository.GetByIdAsNoTrackingAsync(id)
            ?? throw new EntityNotFoundException($"Schedule with id: {id} was not found");
        
        return schedule;
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        var allSchedules = await _scheduleRepository.GetAllAsync();
        if (allSchedules is null)
        {
            return new List<object>();
        }

        return allSchedules
            .Where(schedule => (string)schedule.GetPropertyValue("Status")! == "Active")
            .ToList();
    }
}