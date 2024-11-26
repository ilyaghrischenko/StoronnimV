using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories;

public interface IGroupPageRepository
    : IRepository<GroupPage>, IIncludable<GroupPage>, IReceivable<GroupPage>
{
    
}