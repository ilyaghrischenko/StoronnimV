using AutoMapper;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.DTO.Responses.GroupPage;

namespace StoronnimV.Application.Services.Controllers;

//TODO: Доделать сервис
public class GroupPageControllerService(
    IGroupPageService groupPageService,
    IMemberService memberService,
    ISocialService socialService,
    IMapper mapper) 
    : IGroupPageControllerService
{
    private readonly IGroupPageService _groupPageService = groupPageService;
    private readonly IMemberService _memberService = memberService;
    private readonly ISocialService _socialService = socialService;
    private readonly IMapper _mapper = mapper;
    
    public async Task<GroupPageFullInfoResponse> GetGroupPageInfoAsync()
    {
        var groupPage = await _groupPageService.GetFirstGroupPageAsync();
        var members = await  _memberService.GetAllAsync();
        
        var groupPageDto = _mapper.Map<GroupPageResponse>(groupPage);
        var membersShort = _mapper.Map<IEnumerable<MemberShortResponse>>(members);
        
        var groupPageFullInfoDto = new GroupPageFullInfoResponse(groupPageDto, membersShort);
        
        return groupPageFullInfoDto;
    }

    public async Task<MemberFullInfoResponse> GetMemberInfoAsync(long memberId)
    {
        var members = await _memberService.GetItemByIdAsync(memberId);
        var socials = await _socialService.GetAllForMemberAsync(memberId);
        
        var memberDto = _mapper.Map<MemberResponse>(members);
        var socialsDto = _mapper.Map<IEnumerable<SocialResponse>>(socials);
        
        var memberFullInfoDto = new MemberFullInfoResponse(memberDto, socialsDto);
        
        return memberFullInfoDto;
    }
}