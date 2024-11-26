using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories.Shared;

public interface IReceivable<T> where T : BaseEntity
{
    Task<T?> GetByIdAsNoTrackingAsync(long id);
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>?> GetAllAsync();
}