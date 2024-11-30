using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Entities.Shared;

namespace StoronnimV.Contracts.Repositories.Shared;

public interface IReceivable<T> where T : BaseEntity
{
    Task<object?> GetByIdAsNoTrackingAsync(long id);
    Task<IEnumerable<object>?> GetAllAsync();
}