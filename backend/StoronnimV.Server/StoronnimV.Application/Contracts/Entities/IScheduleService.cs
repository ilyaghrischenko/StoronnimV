using StoronnimV.Application.Contracts.Entities.Shared;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Domain.Projections.Schedule;

namespace StoronnimV.Application.Contracts.Entities;

public interface IScheduleService
    : IGetByIdService<ScheduleFullProjection>, IPaginationService<ScheduleShortProjection>
{
    Task UpdateStatusesAsync(CancellationToken ct);
    Task AddScheduleAsync(ScheduleAdditionRequest request, CancellationToken ct);
    Task DeleteScheduleAsync(long id, CancellationToken ct);
    Task UpdateScheduleAsync(ScheduleEditRequest request, CancellationToken ct);
    Task UpdateSchedulePhotoAsync(PhotoEditRequest request, CancellationToken ct);
}