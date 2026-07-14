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
using StoronnimV.Domain.Projections.News;

namespace StoronnimV.Application.Services.Entities;

/// <summary>
/// Сервис для проверки полученных данных, полученых с репозитория. Так же используется сортировка
/// </summary>
/// <param name="newsRepository"></param>
public class NewsService(
    INewsRepository newsRepository,
    IVideoRepository videoRepository,
    IMediaStorageService mediaStorageService) : INewsService
{
    public async Task<NewsFullProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        NewsFullProjection newsItem = await newsRepository.GetByIdAsNoTrackingAsync(id, ct)
                                      ?? throw new EntityNotFoundException($"News with {nameof(id)}: {id} was not found");

        return newsItem;
    }

    public async Task<PaginationResult<NewsPaginationProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct,
        params object[] args)
    {
        if (page <= 0 || pageSize <= 0)
        {
            throw new PaginationException("Invalid pagination parameters");
        }

        int totalCount = await newsRepository.GetTotalCountAsync(ct);
        int totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling((double)totalCount / pageSize);
        IEnumerable<NewsPaginationProjection> items = totalCount == 0
            ? []
            : await newsRepository.GetForPageAsync(page, ct, pageSize) ?? [];

        return new PaginationResult<NewsPaginationProjection>
        {
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalCount,
            Items = items.ToList()
        };
    }

    /// <summary>
    /// News item addition to the database
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    public async Task AddNewsItemAsync(NewsItemAdditionRequest request, CancellationToken ct)
    {
        Video? newsVideo = null;
        if (request.VideoId != null)
        {
            newsVideo = await videoRepository.GetByIdAsync(request.VideoId.Value, ct);
        }

        News newsItem = new()
        {
            Title = request.Title,
            Description = request.Description,
            Video = newsVideo,
            Priority = Enum.Parse<NewsPriority>(request.Priority),
            Date = request.Date,
        };

        if (request.Photo is null)
        {
            await newsRepository.AddAsync(newsItem, ct);
            return;
        }

        await mediaStorageService.CreateAsync(
            request.Photo,
            MediaKind.Photo,
            "news",
            photoUrl =>
            {
                newsItem.Photo = photoUrl;
                return newsRepository.AddAsync(newsItem, ct);
            },
            ct);
    }
    
    /// <summary>
    /// News item deletion from the database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteNewsItemAsync(long id, CancellationToken ct)
    {
        News? newsItem = await newsRepository.GetByIdAsync(id, ct);

        if (newsItem is null)
        {
            throw new EntityNotFoundException($"NewsItem with {nameof(id)}: {id} was not found");
        }

        await mediaStorageService.DeleteAsync(
            MediaKind.Photo,
            newsItem.Photo,
            () => newsRepository.DeleteAsync(newsItem, ct),
            ct);
    }
    
    public async Task EditNewsItemAsync(NewsItemEditRequest request, CancellationToken ct)
    {
        News? newsItem = await newsRepository.GetByIdAsync(request.Id, ct);

        if (newsItem is null)
        {
            throw new EntityNotFoundException($"NewsItem with {nameof(request.Id)}: {request.Id} was not found");
        }

        await newsRepository.UpdateAsync(newsItem, () =>
        {
            newsItem.Title = request.Title;
            newsItem.Description = request.Description;
            newsItem.Priority = Enum.Parse<NewsPriority>(request.Priority);
            newsItem.Date = request.Date;
        }, ct);
    }

    public async Task EditNewsItemPhotoAsync(PhotoEditRequest photoEditRequest, CancellationToken ct)
    {
        News? newsItem = await newsRepository.GetByIdAsync(photoEditRequest.Id, ct);

        if (newsItem is null)
        {
            throw new EntityNotFoundException($"NewsItem with {nameof(photoEditRequest.Id)}: {photoEditRequest.Id} was not found");
        }

        await mediaStorageService.ReplaceAsync(
            photoEditRequest.Photo,
            MediaKind.Photo,
            "news",
            newsItem.Photo,
            photoUrl => newsRepository.UpdateAsync(newsItem, () => newsItem.Photo = photoUrl, ct),
            ct);
    }

    public async Task EditNewsItemVideoAsync(EntityVideoEditRequest videoEditRequest, CancellationToken ct)
    {
        News? newsItem = await newsRepository.GetByIdAsync(videoEditRequest.Id, ct);

        if (newsItem is null)
        {
            throw new EntityNotFoundException($"NewsItem with {nameof(videoEditRequest.Id)}: {videoEditRequest.Id} was not found");
        }
        
        Video? video = await videoRepository.GetByIdAsync(videoEditRequest.VideoId, ct);

        if (video is null)
        {
            throw new EntityNotFoundException($"Video with {nameof(videoEditRequest.VideoId)}: {videoEditRequest.VideoId} was not found");
        }

        await newsRepository.UpdateAsync(newsItem, () => newsItem.Video = video, ct);
    }
    
    public async Task DeleteNewsItemPhotoAsync(long id, CancellationToken ct)
    {
        News? newsItem = await newsRepository.GetByIdAsync(id, ct);

        if (newsItem is null)
        {
            throw new EntityNotFoundException($"NewsItem with {nameof(id)}: {id} was not found");
        }
        
        await mediaStorageService.DeleteAsync(
            MediaKind.Photo,
            newsItem.Photo,
            () => newsRepository.UpdateAsync(newsItem, () => newsItem.Photo = null, ct),
            ct);
    }

    public async Task DeleteNewsItemVideoAsync(long id, CancellationToken ct)
    {
        News? newsItem = await newsRepository.GetByIdAsync(id, ct);

        if (newsItem is null)
        {
            throw new EntityNotFoundException($"NewsItem with {nameof(id)}: {id} was not found");
        }
        
        await newsRepository.UpdateAsync(newsItem, () => newsItem.Video = null, ct);
    }
}
