using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories;

public interface ISocialRepository
    : IRepository<Social>, IIncludable<Social>, IReceivable<Social>
{
    
}