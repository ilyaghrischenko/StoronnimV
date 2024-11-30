using StoronnimV.Contracts.Services.Entities.Shared;

namespace StoronnimV.Contracts.Services.Entities;

public interface INewsService : IPaginationService
{
    Task<object> GetItemByIdAsync(long id);
    Task<IEnumerable<object>> GetAllAsync();
}