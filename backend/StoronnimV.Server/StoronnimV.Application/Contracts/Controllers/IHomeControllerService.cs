using StoronnimV.Application.DTO.Responses.HomePage;
using StoronnimV.Application.DTO.Responses.Video;

namespace StoronnimV.Application.Contracts.Controllers;

public interface IHomeControllerService
{
    Task<IEnumerable<NewsHomeResponse>> GetMainNewsAsync(int count, CancellationToken ct);
    Task<ScheduleHomeResponse> GetNearestScheduleAsync(CancellationToken ct);
    Task<VideoPageResponse> GetPromotionVideoAsync(CancellationToken ct);
}