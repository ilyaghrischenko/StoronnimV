using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories.Shared;

public interface IReceivable<T> where T : BaseEntity
{
    Task<object?> GetByIdAsNoTrackingAsync(long id);
    Task<object?> GetByIdAsync(long id);
    Task<IEnumerable<object>?> GetAllAsync();
}