using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Responses.Shared;
using StoronnimV.Application.DTO.Responses.Video;

namespace StoronnimV.Api.Controllers;

/// <summary>
/// Контроллер для страницы 'Видео', он позволяет доставать нужные данные для отображения
/// </summary>
/// <param name="videoControllerService"></param>
[EnableRateLimiting("UserLimitPerMinute")]
[Route("api/videos")]
[ApiController]
public class VideoController(
    IVideoControllerService videoControllerService)
    : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<ActionResult<VideoPageResponse>> GetVideo([FromRoute] long id, CancellationToken ct)
    {
        VideoPageResponse video = await videoControllerService.GetItemByIdAsync(id, ct);

        return Ok(video);
    }

    [HttpGet("page/{type}/{page:int}")]
    public async Task<ActionResult<PaginationResponse<VideoPageResponse>>> GetVideosForPage
        ([FromRoute] int page, [FromRoute] string type, CancellationToken ct, [FromQuery] int pageSize = 5)
    {
        var videosPaginationResponse = await videoControllerService.GetForPageAsync(page, pageSize, ct, type);

        return Ok(videosPaginationResponse);
    }
}
