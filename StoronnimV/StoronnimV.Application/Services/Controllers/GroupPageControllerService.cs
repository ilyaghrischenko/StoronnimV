using AutoMapper;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.DTO.Responses.GroupPage;

namespace StoronnimV.Application.Services.Controllers;

//TODO: Доделать сервис
public class GroupPageControllerService(
    IGroupPageService groupPageService,
    IMemberService memberService,
    IMapper mapper) 
    : IGroupPageControllerService
{
    private readonly IGroupPageService _groupPageService = groupPageService;
    private readonly IMemberService _memberService = memberService;
    private readonly IMapper _mapper = mapper;
    
    public async Task<GrouPageFullInfoResponse> GetGroupPageInfoAsync()
    {
        var groupPage = await _groupPageService.GetFirstGroupPageAsync();
        var members = await  _memberService.GetAllAsync();
        
        var groupPageDto = _mapper.Map<GroupPageResponse>(groupPage);
        var membersShort = _mapper.Map<IEnumerable<MemberShortResponse>>(members);
        
        var groupPageFullInfoDto = new GrouPageFullInfoResponse(groupPageDto, membersShort);
        
        return groupPageFullInfoDto;
    }
}