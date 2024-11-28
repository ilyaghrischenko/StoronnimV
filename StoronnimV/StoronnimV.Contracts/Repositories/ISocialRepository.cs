using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Contracts.Repositories;

public interface ISocialRepository
    : IRepository<Social>, IIncludable<Social>, IReceivable<Social>
{
    
}