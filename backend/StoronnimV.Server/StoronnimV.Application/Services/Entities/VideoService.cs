using System.Runtime.ExceptionServices;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Models;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Application.Services.Entities;

public class VideoService(
    IVideoRepository videoRepository,
    IMediaStorageService mediaStorageService) : IVideoService
{
    public async Task<VideoFullProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        VideoFullProjection video = await videoRepository.GetByIdAsNoTrackingAsync(id, ct)
                                    ?? throw new EntityNotFoundException($"Video with {nameof(id)}: {id} was not found");

        return video;
    }

    public async Task<PaginationResult<VideoFullProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct, params object[] args)
    {
        string type = (string)args[0];
        
        if (page <= 0)
        {
            throw new PaginationException("invalid page number");
        }

        int totalCount = await videoRepository.GetTotalCountAsync(ct, type);

        try
        {
            if (totalCount == 0)
            {
                throw new PaginationException(string.Empty);
            }

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var items = await videoRepository.GetForPageAsync(page, ct, pageSize, type);

            if (items is null || !items.Any())
            {
                throw new PaginationException(string.Empty);
            }

            var sortedItems = items.ToList();

            PaginationResult<VideoFullProjection> paginationResult = new()
            {
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalCount,
                Items = sortedItems
            };

            return paginationResult;
        }
        catch (PaginationException)
        {
            return new PaginationResult<VideoFullProjection>
            {
                CurrentPage = page,
                TotalPages = 0,
                TotalItems = 0,
                Items = []
            };
        }
    }
    
    /// <summary>
    /// Video addition to database
    /// </summary>
    /// <param name="request">VideoAdditionRequest</param>
    /// <param name="ct">CancellationToken</param>
    public async Task AddVideoAsync(VideoAdditionRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse(request.Type, out VideoType type))
            throw new ArgumentException("Invalid video type");
        
        Video? oldPromotion = type == VideoType.Promotion
            ? await videoRepository.GetPromotionVideoAsync(ct)
            : null;
        StoredMedia uploaded = await mediaStorageService.UploadAsync(
            request.Url,
            MediaKind.Video,
            "video",
            ct);

        Video video = new()
        {
            Title = request.Title,
            Url = uploaded.Url,
            BlobName = uploaded.BlobName,
            Type = type
        };

        try
        {
            if (oldPromotion is null)
            {
                await videoRepository.AddAsync(video, ct);
            }
            else
            {
                await videoRepository.ReplacePromotionAsync(oldPromotion, video, ct);
            }
        }
        catch (Exception exception)
        {
            await RollBackUploadedBlobAsync(uploaded, exception);
        }

        if (oldPromotion is not null)
        {
            await mediaStorageService.DeleteByBlobNameAsync(
                MediaKind.Video,
                oldPromotion.BlobName,
                () => Task.CompletedTask,
                ct);
        }
    }
    
    
    /// <summary>
    /// Video deletion from database
    /// </summary>
    /// <param name="id">long</param>
    /// <param name="ct">CancellationToken</param>
    /// <exception cref="EntityNotFoundException">EntityNotFoundException</exception>
    public async Task DeleteVideoAsync(long id, CancellationToken ct)
    {
        Video? video = await videoRepository.GetByIdAsync(id, ct);

        if (video is null)
        {
            throw new EntityNotFoundException($"Video with {nameof(id)}: {id} was not found");
        }

        await mediaStorageService.DeleteByBlobNameAsync(
            MediaKind.Video,
            video.BlobName,
            () => videoRepository.DeleteAsync(video, ct),
            ct);
    }

    public async Task UpdateVideoAsync(VideoEditRequest request, CancellationToken ct)
    {
        Video? video = await videoRepository.GetByIdAsync(request.Id, ct);
        
        if (video is null)
        {
            throw new EntityNotFoundException($"Video with {nameof(request.Id)}: {request.Id} was not found");
        }
        
        await videoRepository.UpdateAsync(video, () =>
        {
            video.Title = request.Title;
            video.Type = Enum.Parse<VideoType>(request.Type);
        }, ct);
    }

    private async Task RollBackUploadedBlobAsync(StoredMedia uploaded, Exception originalException)
    {
        try
        {
            await mediaStorageService.DeleteUploadedAsync(uploaded, CancellationToken.None);
        }
        catch (Exception compensationException)
        {
            throw new MediaConsistencyException(
                uploaded.ContainerName,
                uploaded.BlobName,
                new AggregateException(originalException, compensationException));
        }

        ExceptionDispatchInfo.Capture(originalException).Throw();
    }

}
