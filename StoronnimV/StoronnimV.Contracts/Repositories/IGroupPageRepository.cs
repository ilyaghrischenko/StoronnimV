using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Contracts.Repositories;

public interface IGroupPageRepository
    : IRepository<GroupPage>, IIncludable<GroupPage>, IReceivable<GroupPage>
{
    
}