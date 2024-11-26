using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories.Shared;

public interface IIncludable<T> where T : BaseEntity
{
    IQueryable<T> ApplyIncludes(IQueryable<T> dbSet);
}