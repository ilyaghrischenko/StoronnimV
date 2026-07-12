using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Domain.Contracts.Database;

public interface IGroupSocialRepository
    :IRepository<GroupSocial>, IGetByIdRepository<GroupSocialProjection>, IGetAllRepository<GroupSocialProjection>
{
    
}