using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StoronnimV.Application.Contracts.Identity;
using StoronnimV.Application.Options;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Application.Services.Identity;

public class JwtBearerService(IOptions<JwtOptions> jwtOptions) : IJwtBearerService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    
    public ClaimsIdentity GetIdentity(Admin admin)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, admin.Id.ToString()),
            new(ClaimsIdentity.DefaultRoleClaimType, admin.Type.ToString())
        };
        
        ClaimsIdentity identity = new(claims, "Token",
            ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        
        return identity;
    }

    public string GetToken(ClaimsIdentity identity)
    {
        DateTime timeNow = DateTime.UtcNow;
        JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: _jwtOptions.ISSUER,
            audience: _jwtOptions.AUDIENCE,
            notBefore: timeNow,
            claims: identity.Claims,
            expires: timeNow.Add(TimeSpan.FromDays(_jwtOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(_jwtOptions.GetKey(), SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}