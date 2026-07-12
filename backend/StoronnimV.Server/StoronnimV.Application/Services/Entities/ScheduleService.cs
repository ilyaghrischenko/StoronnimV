using System.Globalization;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Models;
using StoronnimV.Domain.Contracts.AzureBlobStorage;
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
    IBlobRepository blobRepository) : IScheduleService
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

        DateTime today = DateTime.UtcNow.Date;

        var schedulesToChange = allSchedules
            .Where(schedule => schedule.Status == ScheduleStatus.Active
                               && schedule.PerformanceDateTime < today)
            .ToList();
        
        schedulesToChange.ForEach(async schedule =>
        {
            await scheduleRepository.UpdateAsync(schedule, () =>
            {
                schedule.Status = ScheduleStatus.Passed;
            }, ct);
        });
    }

    public async Task<PaginationResult<ScheduleShortProjection>> GetForPageAsync(int page, int pageSize,
        CancellationToken ct, params object[] args)
    {
        if (page <= 0)
        {
            throw new PaginationException("Invalid page number");
        }

        int totalCount = await scheduleRepository.GetTotalCountAsync(ct);

        try
        {
            if (totalCount == 0)
            {
                throw new PaginationException(string.Empty);
            }

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var items = await scheduleRepository.GetForPageAsync(page, ct, pageSize);

            if (items is null || !items.Any())
            {
                throw new PaginationException(string.Empty);
            }

            PaginationResult<ScheduleShortProjection> respone = new()
            {
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalCount,
                Items = items.ToList()
            };

            return respone;
        }
        catch (PaginationException)
        {
            return new PaginationResult<ScheduleShortProjection>
            {
                CurrentPage = page,
                TotalPages = 0,
                TotalItems = 0,
                Items = []
            };
        }
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
            PerformanceDateTime = DateTime.SpecifyKind(
                DateTime.Parse(request.PerformanceDateTime, CultureInfo.InvariantCulture),
                DateTimeKind.Utc),
            Description = request.Description,
            Location = request.Location,
            Photo = string.Empty,
            Status = Enum.Parse<ScheduleStatus>(request.Status)
        };

        await scheduleRepository.AddAsync(schedule, ct);

        if (request.Photo != null)
        {
            string extension = Path.GetExtension(request.Photo.FileName);
            string photoUrl = await blobRepository.AddFileAndGetUrlAsync("storonnimv-photo", $"schedule-{schedule.Id}{extension}",
                request.Photo.OpenReadStream(), ct);
            await scheduleRepository.UpdateAsync(schedule, () => schedule.Photo = photoUrl, ct);
        }
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

        await scheduleRepository.DeleteAsync(schedule, ct);

        if (schedule.Photo != null)
        {
            await blobRepository.DeleteAllFilesByNameAsync("storonnimv-photo", $"schedule-{id}", ct);
        }
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
            schedule.PerformanceDateTime = DateTime.SpecifyKind(DateTime.ParseExact(request.PerformanceDateTime, "dd.MM.yyyy HH:mm",
                CultureInfo.InvariantCulture),
                DateTimeKind.Utc);
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

        string scheduleBlobName = $"schedule-{schedule.Id}";

        await blobRepository.DeleteAllFilesByNameAsync("storonnimv-photo", scheduleBlobName, ct);

    
        string extension = Path.GetExtension(request.Photo.FileName);
        string schedulePhotoUrl = await blobRepository.AddFileAndGetUrlAsync
            ("storonnimv-photo", $"{scheduleBlobName}{extension}", request.Photo.OpenReadStream(), ct);

        await scheduleRepository.UpdateAsync(schedule, () => schedule.Photo = schedulePhotoUrl, ct);
    }
}