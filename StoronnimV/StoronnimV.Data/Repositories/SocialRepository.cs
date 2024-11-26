using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data.Repositories;

public class SocialRepository(IDbContextFactory<StoronnimVContext> contextFactory) : 
    Repository<Social>(contextFactory), ISocialRepository
{
    protected override IQueryable<Social> ApplyIncludes(IQueryable<Social> dbSet)
    {
        return base.ApplyIncludes(dbSet);
    }
}