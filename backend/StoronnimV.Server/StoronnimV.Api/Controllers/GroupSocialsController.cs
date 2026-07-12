using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Responses;

namespace StoronnimV.Api.Controllers;

[EnableRateLimiting("UserLimitPerMinute")]
[Route("api/group-socials")]
[ApiController]
public class GroupSocialsController(IGroupSocialsControllerService groupSocialsControllerService) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<ActionResult<GroupSocialResponse>> GetGroupSocial([FromRoute] long id, CancellationToken ct)
    {
        GroupSocialResponse groupSocial = await groupSocialsControllerService.GetItemByIdAsync(id, ct);
        
        return Ok(groupSocial);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupSocialResponse>>> GetAllGroupSocials(CancellationToken ct)
    {
        var groupSocials = await groupSocialsControllerService.GetAllAsync(ct);
        
        return Ok(groupSocials);
    }
}