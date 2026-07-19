using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Models;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Schedule;

namespace StoronnimV.Application.Services.Entities;

/// <summary>
/// Сервис для проверки полученных данных, полученых с репозитория
/// </summary>
/// <param name="scheduleRepository"></param>
public class ScheduleService(
    IScheduleRepository scheduleRepository,
    IMediaStorageService mediaStorageService) : IScheduleService
{
    public async Task<ScheduleFullProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        ScheduleFullProjection schedule = await scheduleRepository.GetByIdAsNoTrackingAsync(id, ct)
                                          ?? throw new EntityNotFoundException(
                                              $"Schedule with {nameof(id)}: {id} was not found");

        return schedule;
    }

    public async Task UpdateStatusesAsync(CancellationToken ct)
    {
        var allSchedules = await scheduleRepository
            .GetAllSchedulesAsync(ct);

        if (allSchedules == null || !allSchedules.Any())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var schedulesToChange = allSchedules
            .Where(schedule => schedule.Status == ScheduleStatus.Active
                               && schedule.PerformanceDateTime < now)
            .ToList();

        foreach (Schedule schedule in schedulesToChange)
        {
            await scheduleRepository.UpdateAsync(schedule, () =>
            {
                schedule.Status = ScheduleStatus.Passed;
            }, ct);
        }
    }

    public async Task<PaginationResult<ScheduleShortProjection>> GetForPageAsync(int page, int pageSize,
        CancellationToken ct, params object[] args)
    {
        if (page <= 0 || pageSize <= 0)
        {
            throw new PaginationException("Invalid pagination parameters");
        }

        int totalCount = await scheduleRepository.GetTotalCountAsync(ct);
        int totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling((double)totalCount / pageSize);
        IEnumerable<ScheduleShortProjection> items = totalCount == 0
            ? []
            : await scheduleRepository.GetForPageAsync(page, ct, pageSize) ?? [];

        return new PaginationResult<ScheduleShortProjection>
        {
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalCount,
            Items = items.ToList()
        };
    }

    /// <summary>
    /// Schedule addition to database
    /// </summary>
    /// <param name="request">ScheduleAdditionRequest</param>
    /// <param name="ct">CancellationToken</param>
    public async Task AddScheduleAsync(ScheduleAdditionRequest request, CancellationToken ct)
    {
        Schedule schedule = new()
        {
            Title = request.Title,
            PerformanceDateTime = DateTime.SpecifyKind(request.PerformanceDateTime, DateTimeKind.Utc),
            Description = request.Description,
            Location = request.Location,
            Photo = string.Empty,
            Status = Enum.Parse<ScheduleStatus>(request.Status)
        };

        if (request.Photo is null)
        {
            await scheduleRepository.AddAsync(schedule, ct);
            return;
        }

        await mediaStorageService.CreateAsync(
            request.Photo,
            MediaKind.Photo,
            "schedule",
            photoUrl =>
            {
                schedule.Photo = photoUrl;
                return scheduleRepository.AddAsync(schedule, ct);
            },
            ct);
    }

    /// <summary>
    /// Schedule deletion from database
    /// </summary>
    /// <param name="id">long</param>
    /// <param name="ct">CancellationToken</param>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteScheduleAsync(long id, CancellationToken ct)
    {
        Schedule? schedule = await scheduleRepository.GetByIdAsync(id, ct);

        if (schedule is null)
        {
            throw new EntityNotFoundException($"Schedule with {nameof(id)}: {id} was not found");
        }

        await mediaStorageService.DeleteAsync(
            MediaKind.Photo,
            schedule.Photo,
            () => scheduleRepository.DeleteAsync(schedule, ct),
            ct);
    }

    public async Task UpdateScheduleAsync(ScheduleEditRequest request, CancellationToken ct)
    {
        Schedule? schedule = await scheduleRepository.GetByIdAsync(request.Id, ct);

        if (schedule is null)
        {
            throw new EntityNotFoundException($"Schedule with {nameof(request.Id)}: {request.Id} was not found");
        }

        await scheduleRepository.UpdateAsync(schedule, () =>
        {
            schedule.Title = request.Title;
            schedule.PerformanceDateTime = DateTime.SpecifyKind(request.PerformanceDateTime, DateTimeKind.Utc);
            schedule.Description = request.Description;
            schedule.Location = request.Location;
        }, ct);
    }

    public async Task UpdateSchedulePhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        Schedule? schedule = await scheduleRepository.GetByIdAsync(request.Id, ct);

        if (schedule is null)
        {
            throw new EntityNotFoundException($"Schedule with {nameof(request.Id)}: {request.Id} was not found");
        }

        await mediaStorageService.ReplaceAsync(
            request.Photo,
            MediaKind.Photo,
            "schedule",
            schedule.Photo,
            photoUrl => scheduleRepository.UpdateAsync(schedule, () => schedule.Photo = photoUrl, ct),
            ct);
    }

    public async Task DeleteSchedulePhotoAsync(long id, CancellationToken ct)
    {
        Schedule? schedule = await scheduleRepository.GetByIdAsync(id, ct);

        if (schedule is null)
        {
            throw new EntityNotFoundException($"Schedule with {nameof(id)}: {id} was not found");
        }

        await mediaStorageService.DeleteAsync(
            MediaKind.Photo,
            schedule.Photo,
            () => scheduleRepository.UpdateAsync(schedule, () => schedule.Photo = null, ct),
            ct);
    }
}
