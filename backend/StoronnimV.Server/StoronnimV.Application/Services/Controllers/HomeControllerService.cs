using AutoMapper;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Home;
using StoronnimV.Application.DTO.Responses.HomePage;
using StoronnimV.Application.DTO.Responses.Video;
using StoronnimV.Domain.Projections.Schedule;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Application.Services.Controllers;

public class HomeControllerService(
    IHomeService homeService,
    IMapper mapper) : IHomeControllerService
{
    public async Task<IEnumerable<NewsHomeResponse>> GetMainNewsAsync(int count, CancellationToken ct)
    {
        var news = await homeService.GetMainNewsForHomePageAsync(count, ct);

        var newsDto = mapper.Map<IEnumerable<NewsHomeResponse>>(news);
        
        return newsDto;
    }

    public async Task<ScheduleHomeResponse> GetNearestScheduleAsync(CancellationToken ct)
    {
        ScheduleShortProjection? schedule = await homeService.GetNearestScheduleForHomePageAsync(ct);
        
        var scheduleDto = mapper.Map<ScheduleHomeResponse>(schedule);
        
        return scheduleDto;
    }

    public async Task<VideoPageResponse> GetPromotionVideoAsync(CancellationToken ct)
    {
        VideoFullProjection? promotionVideo = await homeService.GetPromotionVideoForHomePageAsync(ct);
        
        var promotionVideoDto = mapper.Map<VideoPageResponse>(promotionVideo);
        
        return promotionVideoDto;
    }
}