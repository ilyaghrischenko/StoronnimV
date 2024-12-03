using AutoMapper;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.DTO.Responses.GroupPage;
using StoronnimV.DTO.Responses.GroupPage.ShortGroupPage;
using StoronnimV.DTO.Responses.GroupPage.ShortMember;

namespace StoronnimV.Application.Services.Controllers;

/// <summary>
/// Сервис для маппинга данных с бд и возвращения контроллеру
/// </summary>
/// <param name="groupPageService"></param>
/// <param name="memberService"></param>
/// <param name="socialService"></param>
/// <param name="mapper"></param>
public class GroupPageControllerService(
    IGroupPageService groupPageService,
    IMemberService memberService,
    ISocialService socialService,
    IMapper mapper) : IGroupPageControllerService
{
    private readonly IGroupPageService _groupPageService = groupPageService;
    private readonly IMemberService _memberService = memberService;
    private readonly ISocialService _socialService = socialService;
    private readonly IMapper _mapper = mapper;
    
    public async Task<GroupPageFullInfoResponse> GetGroupPageInfoAsync()
    {
        var groupPageTask = _groupPageService.GetFirstGroupPageAsync();
        var membersTask = _memberService.GetAllAsync();
        
        await Task.WhenAll(groupPageTask, membersTask);
        
        var groupPage = await groupPageTask;
        var members = await membersTask;
        
        var groupPageDto = _mapper.Map<GroupPageResponse>(groupPage);
        var membersShort = _mapper.Map<IEnumerable<MemberShortResponse>>(members);
        
        var groupPageFullInfoDto = new GroupPageFullInfoResponse(groupPageDto, membersShort);
        return groupPageFullInfoDto;
    }

    public async Task<MemberFullInfoResponse> GetMemberInfoAsync(long memberId)
    {
        var memberTask = _memberService.GetItemByIdAsync(memberId);
        var socialsTask = _socialService.GetAllForMemberAsync(memberId);
        
        await Task.WhenAll(memberTask, socialsTask);
        
        var member = await memberTask;
        var socials = await socialsTask;
        
        var memberDto = _mapper.Map<MemberResponse>(member);
        var socialsDto = _mapper.Map<IEnumerable<SocialResponse>>(socials);
        
        var memberFullInfoDto = new MemberFullInfoResponse(memberDto, socialsDto);
        return memberFullInfoDto;
    }
}