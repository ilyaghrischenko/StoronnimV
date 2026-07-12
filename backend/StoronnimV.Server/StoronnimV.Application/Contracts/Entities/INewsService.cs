using StoronnimV.Application.Contracts.Entities.Shared;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Domain.Projections.News;

namespace StoronnimV.Application.Contracts.Entities;

public interface INewsService
    : IPaginationService<NewsPaginationProjection>, IGetByIdService<NewsFullProjection>
{
    Task AddNewsItemAsync(NewsItemAdditionRequest request, CancellationToken ct);
    Task DeleteNewsItemAsync(long id, CancellationToken ct);
    
    Task EditNewsItemAsync(NewsItemEditRequest request, CancellationToken ct);
    Task EditNewsItemPhotoAsync(PhotoEditRequest photoEditRequest, CancellationToken ct);
    Task EditNewsItemVideoAsync(EntityVideoEditRequest videoEditRequest, CancellationToken ct);
    
    Task DeleteNewsItemPhotoAsync(long id, CancellationToken ct);
    Task DeleteNewsItemVideoAsync(long id, CancellationToken ct);
}