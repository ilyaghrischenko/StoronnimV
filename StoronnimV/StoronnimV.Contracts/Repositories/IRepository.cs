using System.Numerics;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>?> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity, Action updateAction);
    Task DeleteAsync(T entity);
}