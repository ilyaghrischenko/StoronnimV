using StoronnimV.Application.Contracts.Entities.Shared;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Application.Contracts.Entities;

public interface IVideoService 
    : IGetByIdService<VideoFullProjection>, IPaginationService<VideoFullProjection>
{
    Task AddVideoAsync(VideoAdditionRequest request, CancellationToken ct);
    Task DeleteVideoAsync(long id, CancellationToken ct);
    Task UpdateVideoAsync(VideoEditRequest request, CancellationToken ct);
}