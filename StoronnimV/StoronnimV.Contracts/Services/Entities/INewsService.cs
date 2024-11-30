using StoronnimV.Contracts.Services.Entities.Shared;

namespace StoronnimV.Contracts.Services.Entities;

public interface INewsService : IReceivableService
{
    Task<IEnumerable<object>> GetForPageAsync(int page);
}