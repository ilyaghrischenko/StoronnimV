using Microsoft.AspNetCore.Identity;
using StoronnimV.Application.Services.Entities;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Admin;

namespace StoronnimV.Tests.Application;

public sealed class SuperAdminServiceTests
{
    [Fact]
    public async Task DeleteBasicAdminAsync_WithSuperAdminId_RejectsWithoutDelete()
    {
        RecordingAdminRepository repository = new(CreateAdmin(1, "owner1", AdminType.SuperAdmin));
        SuperAdminService service = CreateService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.DeleteBasicAdminAsync(1, CancellationToken.None));

        Assert.Equal(0, repository.DeleteCalls);
    }

    [Fact]
    public async Task EditBasicAdminLoginAsync_WithSuperAdminId_RejectsWithoutUpdate()
    {
        RecordingAdminRepository repository = new(CreateAdmin(1, "owner1", AdminType.SuperAdmin));
        SuperAdminService service = CreateService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.EditBasicAdminLoginAsync(1, "owner2", CancellationToken.None));

        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task EditBasicAdminPasswordAsync_WithSuperAdminId_RejectsWithoutUpdate()
    {
        RecordingAdminRepository repository = new(CreateAdmin(1, "owner1", AdminType.SuperAdmin));
        SuperAdminService service = CreateService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.EditBasicAdminPasswordAsync(1, "AAAaa11111", "BBBbb22222", CancellationToken.None));

        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task AddBasicAdminAsync_WithSuperAdminLogin_RejectsWithoutAdd()
    {
        RecordingAdminRepository repository = new(CreateAdmin(1, "owner1", AdminType.SuperAdmin));
        SuperAdminService service = CreateService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddBasicAdminAsync("owner1", "AAAaa11111", CancellationToken.None));

        Assert.Equal(0, repository.AddCalls);
    }

    [Fact]
    public async Task EditBasicAdminPasswordAsync_WithCorrectOldPassword_UpdatesPassword()
    {
        PasswordHasher<Admin> passwordHasher = new();
        Admin basicAdmin = CreateAdmin(1, "admin1", AdminType.Basic);
        basicAdmin.Password = passwordHasher.HashPassword(basicAdmin, "AAAaa11111");
        RecordingAdminRepository repository = new(basicAdmin);
        SuperAdminService service = new(repository, passwordHasher);

        await service.EditBasicAdminPasswordAsync(
            1, "AAAaa11111", "BBBbb22222", CancellationToken.None);

        Assert.Equal(1, repository.UpdateCalls);
        Assert.NotEqual(
            PasswordVerificationResult.Failed,
            passwordHasher.VerifyHashedPassword(basicAdmin, basicAdmin.Password, "BBBbb22222"));
    }

    private static SuperAdminService CreateService(RecordingAdminRepository repository) =>
        new(repository, new PasswordHasher<Admin>());

    private static Admin CreateAdmin(long id, string login, AdminType type) => new()
    {
        Id = id,
        Login = login,
        Password = "hashed-password",
        Type = type
    };

    private sealed class RecordingAdminRepository(params Admin[] admins) : IAdminRepository
    {
        private readonly List<Admin> _admins = admins.ToList();

        public int AddCalls { get; private set; }
        public int UpdateCalls { get; private set; }
        public int DeleteCalls { get; private set; }

        public Task<Admin?> GetByIdAsync(long id, CancellationToken ct) =>
            Task.FromResult(_admins.SingleOrDefault(admin => admin.Id == id));

        public Task AddAsync(Admin entity, CancellationToken ct)
        {
            AddCalls++;
            entity.Id = _admins.Count == 0 ? 1 : _admins.Max(admin => admin.Id) + 1;
            _admins.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Admin entity, Action updateAction, CancellationToken ct)
        {
            UpdateCalls++;
            updateAction();
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Admin entity, CancellationToken ct)
        {
            DeleteCalls++;
            _admins.Remove(entity);
            return Task.CompletedTask;
        }

        public Task<AdminProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct) =>
            Task.FromResult<AdminProjection?>(null);

        public Task<Admin?> GetByLoginAsync(string login, CancellationToken ct) =>
            Task.FromResult(_admins.SingleOrDefault(admin => admin.Login == login));

        public Task<IEnumerable<BasicAdminProjection>?> GetAllBasicAdminsAsync(CancellationToken ct) =>
            Task.FromResult<IEnumerable<BasicAdminProjection>?>(_admins
                .Where(admin => admin.Type == AdminType.Basic)
                .Select(admin => new BasicAdminProjection { Id = admin.Id, Login = admin.Login })
                .ToList());
    }
}
