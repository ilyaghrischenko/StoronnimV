using StoronnimV.Application.Services.Background;
using StoronnimV.Application.Services.Entities;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Schedule;

namespace StoronnimV.Tests.Application;

public sealed class ScheduleStatusUpdaterTests
{
    [Fact]
    public async Task UpdateStatusesAsync_WaitsForEveryExpiredUpdate()
    {
        RecordingScheduleRepository repository = new(
        [
            CreateSchedule(1, DateTime.UtcNow.AddDays(-2)),
            CreateSchedule(2, DateTime.UtcNow.AddDays(-1))
        ])
        {
            BlockUpdates = true
        };
        ScheduleStatusUpdaterService job = new(new ScheduleService(repository, null!));

        Task updateTask = job.UpdateScheduleStatusesAsync(CancellationToken.None);
        await repository.FirstUpdateStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        try
        {
            Assert.False(updateTask.IsCompleted);
        }
        finally
        {
            repository.ReleaseUpdates();
        }

        await updateTask;
        Assert.Equal(2, repository.CompletedUpdates);
    }

    [Fact]
    public async Task UpdateStatusesAsync_UpdatesOnlyExpiredActiveSchedules_AndIsIdempotent()
    {
        Schedule expiredToday = CreateSchedule(1, DateTime.UtcNow.Date);
        Schedule expiredEarlier = CreateSchedule(2, DateTime.UtcNow.AddDays(-2));
        Schedule future = CreateSchedule(3, DateTime.UtcNow.AddDays(1));
        Schedule cancelled = CreateSchedule(4, DateTime.UtcNow.AddDays(-2), ScheduleStatus.Cancelled);
        Schedule alreadyPassed = CreateSchedule(5, DateTime.UtcNow.AddDays(-2), ScheduleStatus.Passed);
        RecordingScheduleRepository repository = new(
            [expiredToday, expiredEarlier, future, cancelled, alreadyPassed]);
        ScheduleService service = new(repository, null!);

        await service.UpdateStatusesAsync(CancellationToken.None);
        await service.UpdateStatusesAsync(CancellationToken.None);

        Assert.Equal(ScheduleStatus.Passed, expiredToday.Status);
        Assert.Equal(ScheduleStatus.Passed, expiredEarlier.Status);
        Assert.Equal(ScheduleStatus.Active, future.Status);
        Assert.Equal(ScheduleStatus.Cancelled, cancelled.Status);
        Assert.Equal(ScheduleStatus.Passed, alreadyPassed.Status);
        Assert.Equal(2, repository.UpdateCalls);
    }

    private static Schedule CreateSchedule(
        long id,
        DateTime performanceDateTime,
        ScheduleStatus status = ScheduleStatus.Active) => new()
    {
        Id = id,
        Title = $"Schedule {id}",
        Description = "Description",
        Location = "Location",
        PerformanceDateTime = performanceDateTime,
        Status = status
    };

    private sealed class RecordingScheduleRepository(IEnumerable<Schedule> schedules) : IScheduleRepository
    {
        private readonly List<Schedule> _schedules = schedules.ToList();
        private readonly TaskCompletionSource _updateGate = new(
            TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource FirstUpdateStarted { get; } = new(
            TaskCreationOptions.RunContinuationsAsynchronously);
        public bool BlockUpdates { get; init; }
        public int UpdateCalls { get; private set; }
        public int CompletedUpdates { get; private set; }

        public void ReleaseUpdates() => _updateGate.TrySetResult();

        public Task<Schedule?> GetByIdAsync(long id, CancellationToken ct) =>
            Task.FromResult(_schedules.SingleOrDefault(schedule => schedule.Id == id));

        public Task AddAsync(Schedule entity, CancellationToken ct) => throw new NotSupportedException();

        public async Task UpdateAsync(Schedule entity, Action updateAction, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            UpdateCalls++;
            FirstUpdateStarted.TrySetResult();
            updateAction();

            if (BlockUpdates)
            {
                await _updateGate.Task.WaitAsync(ct);
            }

            CompletedUpdates++;
        }

        public Task DeleteAsync(Schedule entity, CancellationToken ct) => throw new NotSupportedException();

        public Task<ScheduleFullProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct) =>
            Task.FromResult<ScheduleFullProjection?>(null);

        public Task<IEnumerable<ScheduleShortProjection>?> GetForPageAsync(
            int page,
            CancellationToken ct,
            int pageSize = 10,
            params object[] args) => Task.FromResult<IEnumerable<ScheduleShortProjection>?>([]);

        public Task<int> GetTotalCountAsync(CancellationToken ct, params object[] args) =>
            Task.FromResult(_schedules.Count);

        public Task<IEnumerable<Schedule>?> GetAllSchedulesAsync(CancellationToken ct) =>
            Task.FromResult<IEnumerable<Schedule>?>(_schedules);

        public Task<ScheduleShortProjection?> GetNearestScheduleForHomePageAsync(CancellationToken ct) =>
            Task.FromResult<ScheduleShortProjection?>(null);
    }
}
