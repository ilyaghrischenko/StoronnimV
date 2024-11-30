using StoronnimV.Contracts.Services.Entities.Shared;

namespace StoronnimV.Contracts.Services.Entities;

public interface IScheduleService
{
    Task<object> GetItemByIdAsync(long id);
    Task<IEnumerable<object>> GetAllAsync();
}