using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Responses.MusicPage;

namespace StoronnimV.Api.Controllers;

[EnableRateLimiting("UserLimitPerMinute")]
[Route("api/music")]
[ApiController]
public class MusicController(IMusicControllerService musicControllerService) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<ActionResult<MusicResponse>> GetMusicPlatform([FromRoute] long id, CancellationToken ct)
    {
        MusicResponse musicPlatform = await musicControllerService.GetItemByIdAsync(id, ct);

        return Ok(musicPlatform);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MusicResponse>>> GetMusicPlatforms(CancellationToken ct)
    {
        var musicPlatforms = await musicControllerService.GetAllAsync(ct);

        return Ok(musicPlatforms);
    }
}