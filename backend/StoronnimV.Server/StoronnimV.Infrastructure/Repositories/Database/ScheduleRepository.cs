using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Schedule;
using StoronnimV.Infrastructure.Repositories.Database.Shared;

namespace StoronnimV.Infrastructure.Repositories.Database;

/// <summary>
/// Репозиторий для получения данных напрямую с бд
/// </summary>
/// <param name="context"></param>
public class ScheduleRepository(StoronnimVContext context)
    : Repository<Schedule>(context), IScheduleRepository
{
    private readonly StoronnimVContext _context = context;

    public async Task<ScheduleFullProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.Schedules;
        var query = ApplyIncludes(dbSet);

        ScheduleFullProjection? result = await query
            .AsNoTracking()
            .Select(schedule => new ScheduleFullProjection
            {
                Id = schedule.Id,
                Photo = schedule.Photo,
                Title = schedule.Title,
                Description = schedule.Description,
                PerformanceDateTime = schedule.PerformanceDateTime,
                Location = schedule.Location,
                Status = schedule.Status
            })
            .FirstOrDefaultAsync(schedule => schedule.Id == id, ct);

        return result;
    }

    public async Task<IEnumerable<Schedule>?> GetAllSchedulesAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.Schedules;

        var result = await dbSet.ToListAsync(ct);

        return result;
    }

    public async Task<ScheduleShortProjection?> GetNearestScheduleForHomePageAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.Schedules;
        var query = ApplyIncludes(dbSet);

        ScheduleShortProjection? result = await query
            .AsNoTracking()
            .Where(schedule => schedule.Status == ScheduleStatus.Active)
            .OrderBy(schedule => schedule.PerformanceDateTime)
            .Select(schedule => new ScheduleShortProjection
            {
                Id = schedule.Id,
                Photo = schedule.Photo,
                Title = schedule.Title,
                PerformanceDateTime = schedule.PerformanceDateTime,
                Location = schedule.Location
            })
            .FirstOrDefaultAsync(ct);

        return result;
    }

    public async Task<IEnumerable<ScheduleShortProjection>?> GetForPageAsync(int page, CancellationToken ct, int pageSize = 10, params object[] args)
    {
        ct.ThrowIfCancellationRequested();

        var dbSet = _context.Schedules;
        var query = ApplyIncludes(dbSet);

        var result = await query
            .AsNoTracking()
            .Where(schedule => schedule.Status == ScheduleStatus.Active)
            .OrderBy(schedule => schedule.PerformanceDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(schedule => new ScheduleShortProjection
            {
                Id = schedule.Id,
                Location = schedule.Location,
                PerformanceDateTime = schedule.PerformanceDateTime,
                Title = schedule.Title,
                Photo = schedule.Photo
            })
            .ToListAsync(ct);

        return result;
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct, params object[] args)
    {
        ct.ThrowIfCancellationRequested();

        int count = await _context.Schedules
            .Where(schedule => schedule.Status == ScheduleStatus.Active)
            .CountAsync(ct);

        return count;
    }
}