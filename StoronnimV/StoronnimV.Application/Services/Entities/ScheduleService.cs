using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

public class ScheduleService(IScheduleRepository scheduleRepository) : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository = scheduleRepository;
    
    //TODO: illia - дописать
    public async Task<object> GetItemByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}