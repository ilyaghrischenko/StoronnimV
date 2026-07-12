using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Requests.Account;

namespace StoronnimV.Api.Controllers;

[EnableRateLimiting("AdminLimitPerMinute")]
[Route("api/account")]
[ApiController]
public class AccountController(
    IAccountControllerService accountControllerService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] LogInRequest request, CancellationToken ct)
    {
        string adminRole = await accountControllerService.LogInAsync(Response, request, ct);

        return Ok(adminRole);
    }
}