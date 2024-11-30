using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

public class ScheduleService(IScheduleRepository scheduleRepository) : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository = scheduleRepository;
    
    //TODO: illia - дописать
    public Task<object> GetItemByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<object>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<object>> GetForPageAsync(int page)
    {
        throw new NotImplementedException();
    }
}