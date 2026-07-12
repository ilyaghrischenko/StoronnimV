using StoronnimV.Domain.Projections.News;
using StoronnimV.Domain.Projections.Schedule;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Application.Contracts.Home;

public interface IHomeService
{
    Task<IEnumerable<NewsHomeProjection>> GetMainNewsForHomePageAsync(int count, CancellationToken ct);
    Task<ScheduleShortProjection?> GetNearestScheduleForHomePageAsync(CancellationToken ct);
    Task<VideoFullProjection?> GetPromotionVideoForHomePageAsync(CancellationToken ct);
}