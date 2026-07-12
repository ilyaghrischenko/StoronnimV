using Microsoft.AspNetCore.Identity;
using StoronnimV.Application.Contracts.Identity;
using StoronnimV.Application.Exceptions;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Application.Services.Identity;

public class AccountService(
    IAdminRepository adminRepository,
    IPasswordHasher<Admin> passwordHasher) : IAccountService
{
    public async Task<Admin> LogInAsync(string login, string password, CancellationToken ct)
    {
        Admin? admin = await adminRepository.GetByLoginAsync(login, ct);

        if (admin is null)
        {
            throw new LogInException($"Admin with {nameof(login)}: {login} was not found");
        }
        
        PasswordVerificationResult verificationResult = passwordHasher.VerifyHashedPassword(admin, admin.Password, password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new LogInException("Wrong password");
        }
        
        return admin;
    }
}