using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data.Repositories;

public class GroupPageRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<GroupPage>(contextFactory), IGroupPageRepository
{
    protected override IQueryable<GroupPage> ApplyIncludes(IQueryable<GroupPage> dbSet)
    {
        return base.ApplyIncludes(dbSet);
    }
}