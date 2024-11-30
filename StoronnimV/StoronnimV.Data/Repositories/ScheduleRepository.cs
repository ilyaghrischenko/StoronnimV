using Microsoft.EntityFrameworkCore;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Data.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Data.Repositories;

public class ScheduleRepository(IDbContextFactory<StoronnimVContext> contextFactory)
    : Repository<Schedule>(contextFactory), IScheduleRepository
{
    private readonly IDbContextFactory<StoronnimVContext> _contextFactory = contextFactory;
    
    public async Task<object?> GetByIdAsNoTrackingAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Schedules;
        var query = ApplyIncludes(dbSet);
        
        return await query
            .AsNoTracking()
            .Select(schedule => new
            {
                Id = schedule.Id,
                Photo = schedule.Photo,
                Title = schedule.Title,
                PerformanceDateTime = schedule.PerformanceDateTime.ToShortDateString(),
                Location = schedule.Location,
                Status = schedule.Status.ToString()
            })
            .FirstOrDefaultAsync(schedule => schedule.Id == id);
    }

    public async Task<IEnumerable<object>?> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dbSet = context.Schedules;
        var query = ApplyIncludes(dbSet);
        
        return await query
            .AsNoTracking()
            .Select(schedule => new
            {
                Id = schedule.Id,
                Photo = schedule.Photo,
                Title = schedule.Title,
                PerformanceDateTime = schedule.PerformanceDateTime.ToShortDateString(),
                Location = schedule.Location,
                Status = schedule.Status.ToString()
            })
            .ToListAsync();
    }
}