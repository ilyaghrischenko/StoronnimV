using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories.Shared;

public interface IReceivable<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>?> GetAllAsync();
}