using StoronnimV.Application.Contracts.Home;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Projections.News;
using StoronnimV.Domain.Projections.Schedule;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Application.Services.Home;

public class HomeService(
    INewsRepository newsRepository,
    IScheduleRepository scheduleRepository,
    IVideoRepository videoRepository) : IHomeService
{
    public async Task<IEnumerable<NewsHomeProjection>> GetMainNewsForHomePageAsync(int count, CancellationToken ct)
    {
        var newsForHomePage = await newsRepository.GetMainNewsForHomePageAsync(count, ct);
        if (newsForHomePage is null || !newsForHomePage.Any())
        {
            return new List<NewsHomeProjection>();
        }
        
        return newsForHomePage
            .ToList();
    }

    public async Task<ScheduleShortProjection?> GetNearestScheduleForHomePageAsync(CancellationToken ct)
    {
        ScheduleShortProjection? schedule = await scheduleRepository.GetNearestScheduleForHomePageAsync(ct);
        
        return schedule;
    }

    public async Task<VideoFullProjection?> GetPromotionVideoForHomePageAsync(CancellationToken ct)
    {
        VideoFullProjection? promotionVideo = await videoRepository.GetPromotionVideoForHomePageAsync(ct);
        
        return promotionVideo;
    }
}