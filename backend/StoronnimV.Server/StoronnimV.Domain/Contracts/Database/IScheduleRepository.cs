using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections.Schedule;

namespace StoronnimV.Domain.Contracts.Database;

public interface IScheduleRepository
    : IRepository<Schedule>, IGetByIdRepository<ScheduleFullProjection>,
        IPaginationRepository<ScheduleShortProjection>
{
    Task<IEnumerable<Schedule>?> GetAllSchedulesAsync(CancellationToken ct);
    Task<ScheduleShortProjection?> GetNearestScheduleForHomePageAsync(CancellationToken ct);
}