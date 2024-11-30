using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Data.Repositories.Shared;

/// <summary>
/// Общий репозиторий (Generic), нужен для круд запросов для каждой из сущностей
/// </summary>
/// <param name="contextFactory"></param>
/// <typeparam name="T">Entity</typeparam>
public class Repository<T>(IDbContextFactory<StoronnimVContext> contextFactory)
    : IRepository<T> where T : BaseEntity
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;

    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> dbSet)
    {
        return dbSet;
    }
    
    public async Task<T?> GetByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Set<T>();
        var query = ApplyIncludes(dbSet);

        return await query
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(T entity)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Set<T>();
        
        await dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity, Action updateAction)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Set<T>();

        dbSet.Update(entity);
        updateAction();
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Set<T>();
        
        dbSet.Remove(entity);
        await context.SaveChangesAsync();
    }
}