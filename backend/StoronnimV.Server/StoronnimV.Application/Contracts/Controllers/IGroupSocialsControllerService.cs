using StoronnimV.Application.Contracts.Controllers.Shared;
using StoronnimV.Application.DTO.Responses;

namespace StoronnimV.Application.Contracts.Controllers;

public interface IGroupSocialsControllerService
    : IGetAllControllerService<GroupSocialResponse>, IGetByIdControllerService<GroupSocialResponse>
{
    
}