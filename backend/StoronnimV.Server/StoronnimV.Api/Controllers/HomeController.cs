using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Responses.HomePage;
using StoronnimV.Application.DTO.Responses.Video;

namespace StoronnimV.Api.Controllers;

[EnableRateLimiting("UserLimitPerMinute")]
[Route("api/home")]
[ApiController]
public class HomeController(
    IHomeControllerService homeControllerService) : ControllerBase
{
    [HttpGet("news/{count:int}")]
    public async Task<ActionResult<IEnumerable<NewsHomeResponse>>> GetMainNews([FromRoute] int count,
        CancellationToken ct)
    {
        var newsDto = await homeControllerService.GetMainNewsAsync(count, ct);

        return Ok(newsDto);
    }

    [HttpGet("schedule")]
    public async Task<ActionResult<ScheduleHomeResponse>> GetNearestSchedule(CancellationToken ct)
    {
        ScheduleHomeResponse scheduleDto = await homeControllerService.GetNearestScheduleAsync(ct);

        return Ok(scheduleDto);
    }

    [HttpGet("video")]
    public async Task<ActionResult<VideoPageResponse>> GetPromotionVideo(CancellationToken ct)
    {
        VideoPageResponse videoDto = await homeControllerService.GetPromotionVideoAsync(ct);

        return Ok(videoDto);
    }
}