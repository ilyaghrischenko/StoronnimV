using System.Security.Claims;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Application.Contracts.Identity;

public interface IJwtBearerService
{
    ClaimsIdentity GetIdentity(Admin admin);
    string GetToken(ClaimsIdentity identity);
}