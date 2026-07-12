using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections.Member;

namespace StoronnimV.Domain.Contracts.Database;

public interface IMemberRepository
    : IRepository<Member>, IGetByIdRepository<MemberFullProjection>, IGetAllRepository<MemberShortProjection>
{
    
}