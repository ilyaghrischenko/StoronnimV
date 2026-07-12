using AutoMapper;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Requests.Entities.Admin;
using StoronnimV.Application.DTO.Responses.Admin;
using StoronnimV.Domain.Projections.Admin;

namespace StoronnimV.Application.Services.Controllers;

public class SuperAdminControllerService(
    ISuperAdminService superAdminService,
    IMapper mapper) : ISuperAdminControllerService
{
    public async Task<IEnumerable<BasicAdminResponse>> GetAllAsync(CancellationToken ct)
    {
        var basicAdminProjections = await superAdminService.GetAllAsync(ct);

        var basicAdminDtos = mapper.Map<IEnumerable<BasicAdminResponse>>(basicAdminProjections);
        return basicAdminDtos;
    }

    public async Task DeleteBasicAdminAsync(long id, CancellationToken ct)
    {
        await superAdminService.DeleteBasicAdminAsync(id, ct);
    }

    public async Task<BasicAdminResponse> AddBasicAdminAsync(CreateBasicAdminRequest request, CancellationToken ct)
    {
        string login = request.Login;
        string unhashedPassword = request.Password;
        
        BasicAdminProjection createdAdminProjection = await superAdminService.AddBasicAdminAsync(login, unhashedPassword, ct);

        var mappedAdmin = mapper.Map<BasicAdminResponse>(createdAdminProjection);
        return mappedAdmin;
    }

    public async Task EditBasicAdminPasswordAsync(long id, EditBasicAdminPasswordRequest passwordRequest,
        CancellationToken ct)
    {
        string oldPassword = passwordRequest.OldPassword;
        string newPassword = passwordRequest.NewPassword;
        
        await superAdminService.EditBasicAdminPasswordAsync(id, oldPassword, newPassword, ct);
    }

    public async Task<BasicAdminResponse> EditBasicAdminLoginAsync(long id, EditBasicAdminLoginRequest loginRequest, CancellationToken ct)
    {
        string newLogin = loginRequest.NewLogin;
        
        BasicAdminProjection changedAdmin = await superAdminService.EditBasicAdminLoginAsync(id, newLogin, ct);
        
        var mappedAdmin = mapper.Map<BasicAdminResponse>(changedAdmin);
        return mappedAdmin;
    }
}