using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories;

public interface IMemberRepository
    : IRepository<Member>, IIncludable<Member>, IReceivable<Member>
{
    
}