using StoronnimV.Domain.Entities;

namespace StoronnimV.Application.Contracts.Identity;

public interface IAccountService
{
    Task<Admin> LogInAsync(string login, string password, CancellationToken ct);
}