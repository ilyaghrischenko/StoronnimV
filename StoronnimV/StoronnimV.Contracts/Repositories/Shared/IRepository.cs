using StoronnimV.Domain.Entities;

namespace StoronnimV.Contracts.Repositories.Shared;

public interface IRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity, Action updateAction);
    Task DeleteAsync(T entity);
}