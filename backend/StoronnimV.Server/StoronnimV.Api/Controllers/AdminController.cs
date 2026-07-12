using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using StoronnimV.Api.Extensions;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.Options;

namespace StoronnimV.Api.Controllers;

/// <summary>
/// Контроллер для админа, он позволяет управлять данными, которые отображаются на страницых (Удалять, изменять)
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[EnableRateLimiting("AdminLimitPerMinute")]
[Route("api/admin")]
[ApiController]
public class AdminController(IAdminControllerService adminControllerService) : ControllerBase
{
    [HttpGet("isAdmin")]
    public ActionResult<bool> IsAdmin()
    {
        return Ok(true);
    }

    [HttpPost("logout")]
    public IActionResult LogOut([FromServices] IOptionsMonitor<CookieSettings> cookieSettings, CancellationToken ct)
    {
        CookieSettings currentCookieSettings = cookieSettings.CurrentValue;
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = currentCookieSettings.HttpOnly,
            Secure = currentCookieSettings.Secure,
            SameSite = Enum.Parse<SameSiteMode>(currentCookieSettings.SameSite),
            Expires = DateTime.UtcNow.AddHours(currentCookieSettings.ExpiresInHours),
            Domain = EnvironmentExtensions.GetEnvironmentVariableOrThrowException("DOMAIN")
        };
        
        Response.Cookies.Delete("Token", cookieOptions);
        return Ok();
    }

    #region DELETE Methods

    [HttpDelete("news/{id:long}")]
    public async Task<IActionResult> DeleteNewsItem([FromRoute] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteNewsItemAsync(id, ct);

        return NoContent();
    }

    [HttpDelete("schedules/{id:long}")]
    public async Task<IActionResult> DeleteSchedule([FromRoute] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteScheduleAsync(id, ct);

        return NoContent();
    }

    [HttpDelete("videos/{id:long}")]
    public async Task<IActionResult> DeleteVideo([FromRoute] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteVideoAsync(id, ct);

        return NoContent();
    }

    [HttpDelete("group/{id:long}")]
    public async Task<IActionResult> DeleteGroup([FromRoute] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteGroupPageAsync(id, ct);

        return NoContent();
    }

    [HttpDelete("group/members/{id:long}")]
    public async Task<IActionResult> DeleteMember([FromRoute] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteMemberAsync(id, ct);

        return NoContent();
    }

    [HttpDelete("music/{id:long}")]
    public async Task<IActionResult> DeleteMusicPlatform([FromRoute] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteMusicPlatformAsync(id, ct);

        return NoContent();
    }

    [HttpDelete("socials/{id:long}")]
    public async Task<IActionResult> DeleteSocial([FromRoute] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteSocialAsync(id, ct);

        return NoContent();
    }

    [HttpDelete("group-socials/{id:long}")]
    public async Task<IActionResult> DeleteGroupSocial([FromRoute] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteGroupSocialAsync(id, ct);
        
        return NoContent();
    }

    #endregion

    #region ADD methods

    [HttpPost("news")]
    public async Task<IActionResult> AddNewsItem([FromForm] NewsItemAdditionRequest request, CancellationToken ct)
    {
        await adminControllerService.AddNewsItemAsync(request, ct);

        return Created();
    }

    [HttpPost("schedules")]
    public async Task<IActionResult> AddSchedule([FromForm] ScheduleAdditionRequest request, CancellationToken ct)
    {
        await adminControllerService.AddScheduleAsync(request, ct);

        return Created();
    }

    [HttpPost("videos")]
    public async Task<IActionResult> AddVideo([FromForm] VideoAdditionRequest request, CancellationToken ct)
    {
        await adminControllerService.AddVideoAsync(request, ct);

        return Created();
    }

    [HttpPost("group")]
    public async Task<IActionResult> AddGroup([FromForm] GroupPageAdditionRequest request, CancellationToken ct)
    {
        await adminControllerService.AddGroupPageAsync(request, ct);

        return Created();
    }

    [HttpPost("group/members")]
    public async Task<IActionResult> AddMember([FromForm] MemberAdditionRequest request, CancellationToken ct)
    {
        await adminControllerService.AddMemberAsync(request, ct);

        return Created();
    }

    [HttpPost("music")]
    public async Task<IActionResult> AddMusicPlatform([FromForm] MusicPlatformAdditionRequest request,
        CancellationToken ct)
    {
        await adminControllerService.AddMusicPlatformAsync(request, ct);

        return Created();
    }

    [HttpPost("socials")]
    public async Task<IActionResult> AddSocial([FromBody] SocialAdditionRequest request, CancellationToken ct)
    {
        await adminControllerService.AddSocialAsync(request, ct);

        return Created();
    }

    [HttpPost("group-socials")]
    public async Task<IActionResult> AddGroupSocial([FromForm] GroupSocialAdditionRequest request, CancellationToken ct)
    {
        await adminControllerService.AddGroupSocialAsync(request, ct);
        
        return Created();
    }

    #endregion

    #region UPDATE methods

    [HttpPatch("news")]
    public async Task<IActionResult> UpdateNewsItem([FromBody] NewsItemEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateNewsItemAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("schedules")]
    public async Task<IActionResult> UpdateSchedule([FromBody] ScheduleEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateScheduleAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("videos")]
    public async Task<IActionResult> UpdateVideo([FromBody] VideoEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateVideoAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("group-pages")]
    public async Task<IActionResult> UpdateGroupPage([FromBody] GroupPageEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateGroupPageAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("group-pages/members")]
    public async Task<IActionResult> UpdateMember([FromBody] MemberEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateMemberAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("music-platforms")]
    public async Task<IActionResult> UpdateMusicPlatform([FromBody] MusicPlatformEditRequest request,
        CancellationToken ct)
    {
        await adminControllerService.UpdateMusicPlatformAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("socials")]
    public async Task<IActionResult> UpdateSocial([FromBody] SocialEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateSocialAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("group-socials")]
    public async Task<IActionResult> UpdateGroupSocial([FromBody] GroupSocialEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateGroupSocialAsync(request, ct);
        
        return NoContent();
    }

    #endregion

    #region UPDATE PHOTO methods
    [HttpPatch("news/photo")]
    public async Task<IActionResult> UpdateNewsItemPhoto([FromForm] PhotoEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateNewsItemPhotoAsync(request, ct);

        return NoContent();
    }
    
    [HttpPatch("news/delete-photo")]
    public async Task<IActionResult> DeleteNewsItemPhoto([FromBody] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteNewsItemPhotoAsync(id, ct);
        
        return NoContent();
    }

    [HttpPatch("schedules/photo")]
    public async Task<IActionResult> UpdateSchedulePhoto([FromForm] PhotoEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateSchedulePhotoAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("group-page/photo")]
    public async Task<IActionResult> UpdateGroupPhoto([FromForm] PhotoEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateGroupPagePhotoAsync(request, ct);

        return NoContent();
    }
    
    [HttpPatch("group-page/members/photo")]
    public async Task<IActionResult> UpdateMemberPhoto([FromForm] PhotoEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateMemberPhotoAsync(request, ct);

        return NoContent();
    }

    [HttpPatch("music-platforms/photo")]
    public async Task<IActionResult> UpdateMusicPlatformPhoto([FromForm] PhotoEditRequest request, CancellationToken ct)
    {
        await adminControllerService.UpdateMusicPlatformPhotoAsync(request, ct);

        return NoContent();
    }
    #endregion

    #region UPDATE VIDEO methods
    [HttpPatch("news/video")]
    public async Task<IActionResult> UpdateNewsItemVideo([FromBody] EntityVideoEditRequest request,
        CancellationToken ct)
    {
        await adminControllerService.UpdateNewsItemVideoAsync(request, ct);

        return NoContent();
    }
    
    [HttpPatch("news/delete-video")]
    public async Task<IActionResult> DeleteNewsItemVideo([FromBody] long id, CancellationToken ct)
    {
        await adminControllerService.DeleteNewsItemVideoAsync(id, ct);
        
        return NoContent();
    }
    #endregion
}