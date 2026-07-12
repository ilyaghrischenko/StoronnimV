using AutoMapper;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Responses;

namespace StoronnimV.Application.Services.Controllers;

public class GroupSocialsControllerService(
    IGroupSocialService groupSocialService,
    IMapper mapper)
    : IGroupSocialsControllerService
{
    public async Task<IEnumerable<GroupSocialResponse>> GetAllAsync(CancellationToken ct)
    {
        var groupSocials = await groupSocialService.GetAllAsync(ct);
        
        var groupSocialsDto = mapper.Map<IEnumerable<GroupSocialResponse>>(groupSocials);
        
        return groupSocialsDto;
    }

    public async Task<GroupSocialResponse> GetItemByIdAsync(long id, CancellationToken ct)
    {
        var groupSocial = await groupSocialService.GetItemByIdAsync(id, ct);
        
        var groupSocialDto = mapper.Map<GroupSocialResponse>(groupSocial);
        
        return groupSocialDto;
    }
}