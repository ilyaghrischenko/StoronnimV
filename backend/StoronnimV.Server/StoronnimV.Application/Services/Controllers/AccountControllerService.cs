using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Identity;
using StoronnimV.Application.DTO.Requests.Account;
using StoronnimV.Application.Options;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Application.Services.Controllers;

public class AccountControllerService(
    IAccountService accountService,
    IJwtBearerService jwtBearerService,
    IOptionsMonitor<CookieSettings> cookieSettings) : IAccountControllerService
{
    private readonly CookieSettings _cookieSettings = cookieSettings.CurrentValue;
    
    public async Task<string> LogInAsync(HttpResponse response, LogInRequest request, CancellationToken ct)
    {
        Admin admin = await accountService.LogInAsync(request.Login, request.Password, ct);
        
        ClaimsIdentity identity = jwtBearerService.GetIdentity(admin);
        string token = jwtBearerService.GetToken(identity);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = _cookieSettings.HttpOnly,
            Secure = _cookieSettings.Secure,
            SameSite = Enum.Parse<SameSiteMode>(_cookieSettings.SameSite),
            Expires = DateTime.UtcNow.AddHours(_cookieSettings.ExpiresInHours),
            Domain = Environment.GetEnvironmentVariable("DOMAIN")
        };
        
        response.Cookies.Append("Token", token, cookieOptions);

        return admin.Type.ToString();
    }
}