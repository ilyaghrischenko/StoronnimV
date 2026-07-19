using Microsoft.AspNetCore.Identity;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.Exceptions;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Admin;

namespace StoronnimV.Application.Services.Entities;

public class SuperAdminService(
    IAdminRepository adminRepository,
    IPasswordHasher<Admin> passwordHasher) : ISuperAdminService
{
    public async Task<IEnumerable<BasicAdminProjection>> GetAllAsync(CancellationToken ct)
    {
        var basicAdmins = await adminRepository.GetAllBasicAdminsAsync(ct);

        return basicAdmins ?? new List<BasicAdminProjection>();
    }

    public async Task DeleteBasicAdminAsync(long id, CancellationToken ct)
    {
        Admin? basicAdmin = await adminRepository.GetByIdAsync(id, ct);

        if (basicAdmin is null)
        {
            throw new EntityNotFoundException($"Basic Admin with {nameof(id)}: {id} was not found");
        }

        ThrowIfNotBasicAdmin(basicAdmin);
        
        await adminRepository.DeleteAsync(basicAdmin, ct);
    }

    public async Task<BasicAdminProjection> AddBasicAdminAsync(string login, string unhashedPassword, CancellationToken ct)
    {
        await ThrowIfLoginAlreadyExistsAsync(login, null, ct);
        
        string hashedPassword = passwordHasher.HashPassword(null!, unhashedPassword);

        Admin newBasicAdmin = new()
        {
            Login = login,
            Password = hashedPassword,
            Type = AdminType.Basic
        };
        
        await adminRepository.AddAsync(newBasicAdmin, ct);

        return new BasicAdminProjection
        {
            Id = newBasicAdmin.Id,
            Login = newBasicAdmin.Login,
        };
    }

    public async Task<BasicAdminProjection> EditBasicAdminLoginAsync(long id, string newLogin, CancellationToken ct)
    {
        Admin? adminToChange = await adminRepository.GetByIdAsync(id, ct);
        
        if (adminToChange is null)
        {
            throw new EntityNotFoundException($"Admin with {nameof(id)}: {id} was not found");
        }

        ThrowIfNotBasicAdmin(adminToChange);
        await ThrowIfLoginAlreadyExistsAsync(newLogin, id, ct);

        await adminRepository.UpdateAsync(adminToChange, () =>
        {
            adminToChange.Login = newLogin;
        }, ct);

        return new BasicAdminProjection
        {
            Id = adminToChange.Id,
            Login = adminToChange.Login
        };
    }

    private async Task ThrowIfLoginAlreadyExistsAsync(
        string login,
        long? editedAdminId,
        CancellationToken ct)
    {
        Admin? adminWithSameLogin = await adminRepository.GetByLoginAsync(login, ct);
        if (adminWithSameLogin is not null && adminWithSameLogin.Id != editedAdminId)
        {
            throw new ArgumentException($"Admin with {nameof(login)}: {login} already exists");
        }
    }

    public async Task EditBasicAdminPasswordAsync(long id, string oldPassword, string newUnhashedPassword,
        CancellationToken ct)
    {
        Admin? adminToChange = await adminRepository.GetByIdAsync(id, ct);
        
        if (adminToChange is null)
        {
            throw new EntityNotFoundException($"Admin with {nameof(id)}: {id} was not found");
        }

        ThrowIfNotBasicAdmin(adminToChange);
        
        PasswordVerificationResult verificationResult = passwordHasher.VerifyHashedPassword(adminToChange, adminToChange.Password, oldPassword);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new ArgumentException("passwords do not match");
        }
        
        string newHashedPassword = passwordHasher.HashPassword(null!, newUnhashedPassword);

        await adminRepository.UpdateAsync(adminToChange, () =>
        {
            adminToChange.Password = newHashedPassword;
        }, ct);
    }

    private static void ThrowIfNotBasicAdmin(Admin admin)
    {
        if (admin.Type != AdminType.Basic)
        {
            throw new ArgumentException("Only Basic Admin accounts can be changed through these endpoints");
        }
    }
}
