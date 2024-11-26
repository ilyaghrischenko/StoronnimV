using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data.Repositories;

public class MemberRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<Member>(contextFactory), IMemberRepository
{
    protected override IQueryable<Member> ApplyIncludes(IQueryable<Member> dbSet)
    {
        return base.ApplyIncludes(dbSet);
    }
}