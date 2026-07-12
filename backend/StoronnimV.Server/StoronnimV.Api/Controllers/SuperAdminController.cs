using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Requests.Entities.Admin;
using StoronnimV.Application.DTO.Responses.Admin;

namespace StoronnimV.Api.Controllers;

[Authorize(Policy = "SuperAdminOnly")]
[EnableRateLimiting("AdminLimitPerMinute")]
[Route("api/super-admin/basic-admins")]
[ApiController]
public class SuperAdminController(ISuperAdminControllerService superAdminControllerService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BasicAdminResponse>>> GetAllBasicAdmins(CancellationToken ct)
    {
        var admins = await superAdminControllerService.GetAllAsync(ct);

        return Ok(admins);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteBasicAdmin([FromRoute] long id, CancellationToken ct)
    {
        await superAdminControllerService.DeleteBasicAdminAsync(id, ct);

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<BasicAdminResponse>> CreateBasicAdmin([FromBody] CreateBasicAdminRequest request,
        CancellationToken ct)
    {
        BasicAdminResponse createdAdmin = await superAdminControllerService.AddBasicAdminAsync(request, ct);

        return Ok(createdAdmin);
    }

    [HttpPatch("{id:long}/login")]
    public async Task<ActionResult<BasicAdminResponse>> EditBasicAdminLogin([FromRoute] long id,
        [FromBody] EditBasicAdminLoginRequest loginRequest, CancellationToken ct)
    {
        BasicAdminResponse changedAdmin =
            await superAdminControllerService.EditBasicAdminLoginAsync(id, loginRequest, ct);

        return Ok(changedAdmin);
    }

    [HttpPatch("{id:long}/password")]
    public async Task<IActionResult> EditBasicAdminPassword([FromRoute] long id,
        [FromBody] EditBasicAdminPasswordRequest passwordRequest, CancellationToken ct)
    {
        await superAdminControllerService.EditBasicAdminPasswordAsync(id, passwordRequest, ct);

        return Ok();
    }
}