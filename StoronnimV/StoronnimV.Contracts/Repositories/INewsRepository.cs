using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Contracts.Repositories;

public interface INewsRepository
    : IRepository<News>, IReceivable<News>
{
    Task<IEnumerable<object>?> GetForPageAsync(int page, int pageSize = 10);
}