using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Video;
using StoronnimV.Infrastructure.Repositories.Database.Shared;

namespace StoronnimV.Infrastructure.Repositories.Database;

/// <summary>
/// Репозиторий для получения данных напрямую с бд
/// </summary>
/// <param name="context"></param>
public class VideoRepository(StoronnimVContext context)
    : Repository<Video>(context), IVideoRepository
{
    private readonly StoronnimVContext _context = context;

    public async Task<VideoFullProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        VideoFullProjection? video = await _context.Videos
            .AsNoTracking()
            .Select(v => new VideoFullProjection
            {
                Id = v.Id,
                Title = v.Title,
                Type = v.Type,
                Url = v.Url
            })
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return video;
    }

    public async Task<VideoFullProjection?> GetPromotionVideoForHomePageAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        VideoFullProjection? promotionVideo = await _context.Videos
            .AsNoTracking()
            .Where(video => video.Type == VideoType.Promotion)
            .Select(video => new VideoFullProjection
            {
                Id = video.Id,
                Title = video.Title,
                Type = video.Type,
                Url = video.Url
            })
            .FirstOrDefaultAsync(ct);
        
        return promotionVideo;
    }

    public async Task<Video?> GetPromotionVideoAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var promotionVideo = await _context.Videos
            .Where(video => video.Type == VideoType.Promotion)
            .FirstOrDefaultAsync(ct);
        
        return promotionVideo;
    }


    public async Task<IEnumerable<VideoFullProjection>?> GetForPageAsync(int page, CancellationToken ct, int pageSize = 10, params object[] args)
    {
        ct.ThrowIfCancellationRequested();

        string type = (string)args[0];
        var typeEnum = Enum.Parse<VideoType>(type);
        
        var videos = await _context.Videos
            .AsNoTracking()
            .Where(video => video.Type == typeEnum)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VideoFullProjection
            {
                Id = v.Id,
                Title = v.Title,
                Type = v.Type,
                Url = v.Url
            })
            .ToListAsync(ct);
        
        return videos;
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct, params object[] args)
    {
        ct.ThrowIfCancellationRequested();

        string type = (string)args[0];
        var typeEnum = Enum.Parse<VideoType>(type);
        
        int count = await _context.Videos
            .AsNoTracking()
            .Where(video => video.Type == typeEnum)
            .CountAsync(ct);
        
        return count;
    }
}