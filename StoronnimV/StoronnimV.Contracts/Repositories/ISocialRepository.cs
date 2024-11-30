using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Contracts.Repositories;

public interface ISocialRepository
    : IRepository<Social>, IReceivableRepository<Social>
{
    public Task<IEnumerable<object>?> GetAllForMemberAsync(long memberId);
}