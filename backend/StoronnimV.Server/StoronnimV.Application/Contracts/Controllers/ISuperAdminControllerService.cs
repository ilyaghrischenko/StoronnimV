using StoronnimV.Application.Contracts.Controllers.Shared;
using StoronnimV.Application.DTO.Requests.Entities.Admin;
using StoronnimV.Application.DTO.Responses.Admin;

namespace StoronnimV.Application.Contracts.Controllers;

public interface ISuperAdminControllerService : IGetAllControllerService<BasicAdminResponse>
{
    Task DeleteBasicAdminAsync(long id, CancellationToken ct);
    Task<BasicAdminResponse> AddBasicAdminAsync(CreateBasicAdminRequest request, CancellationToken ct);
    Task EditBasicAdminPasswordAsync(long id, EditBasicAdminPasswordRequest passwordRequest, CancellationToken ct);
    Task<BasicAdminResponse> EditBasicAdminLoginAsync(long id, EditBasicAdminLoginRequest request, CancellationToken ct);
}