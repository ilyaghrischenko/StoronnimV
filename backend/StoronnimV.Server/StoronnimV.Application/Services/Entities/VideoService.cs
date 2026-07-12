using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Models;
using StoronnimV.Domain.Contracts.AzureBlobStorage;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Application.Services.Entities;

public class VideoService(
    IVideoRepository videoRepository,
    IBlobRepository blobRepository) : IVideoService
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
        
        await DeleteIfVideoToAddIsPromotion(type, ct);

        string videoBlobName = $"video-{Guid.NewGuid()}.mp4";
        string videoUrl = await blobRepository.AddFileAndGetUrlAsync("storonnimv-video", videoBlobName, request.Url.OpenReadStream(), ct);
        
        Video video = new()
        {
            Title = request.Title,
            Url = videoUrl,
            BlobName = videoBlobName,
            Type = type
        };
        
        await videoRepository.AddAsync(video, ct);
    }

    private async Task DeleteIfVideoToAddIsPromotion(VideoType type, CancellationToken ct)
    {
        if (type != VideoType.Promotion)
        {
            return;
        }

        var promotionVideo = await videoRepository.GetPromotionVideoAsync(ct);
        if (promotionVideo is null)
        {
            return;
        }
        
        await DeleteVideoAsync(promotionVideo.Id, ct);
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

        await videoRepository.DeleteAsync(video, ct);

        await blobRepository.DeleteFileAsync("storonnimv-video", video.BlobName, ct);
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
}