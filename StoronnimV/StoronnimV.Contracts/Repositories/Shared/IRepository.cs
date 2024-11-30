using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Entities.Shared;

namespace StoronnimV.Contracts.Repositories.Shared;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(long id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity, Action updateAction);
    Task DeleteAsync(T entity);
}