using StoronnimV.Domain.Entities.Shared;

namespace StoronnimV.Domain.Contracts.Database.Shared;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(long id, CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task UpdateAsync(T entity, Action updateAction, CancellationToken ct);
    Task DeleteAsync(T entity, CancellationToken ct);
}