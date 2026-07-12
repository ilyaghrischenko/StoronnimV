using AutoMapper;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Responses.GroupPage;
using StoronnimV.Application.DTO.Responses.GroupPage.ShortGroupPage;
using StoronnimV.Application.DTO.Responses.GroupPage.ShortMember;
using StoronnimV.Domain.Projections;

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
    IMapper mapper) : IGroupPageControllerService
{
    public async Task<GroupPageFullInfoResponse> GetGroupPageInfoAsync(CancellationToken ct)
    {
        GroupPageProjection groupPage = await groupPageService.GetFirstGroupPageAsync(ct);
        var members = await memberService.GetAllAsync(ct);
        
        var groupPageDto = mapper.Map<GroupPageResponse>(groupPage);
        var membersShort = mapper.Map<IEnumerable<MemberShortResponse>>(members);

        GroupPageFullInfoResponse groupPageFullInfoDto = new()
        {
            GroupPage = groupPageDto,
            Members = membersShort
        };
        
        return groupPageFullInfoDto;
    }

    public async Task<MemberFullInfoResponse> GetMemberAsync(long memberId, CancellationToken ct)
    {
        var member = await memberService.GetItemByIdAsync(memberId, ct);
        
        var memberDto = mapper.Map<MemberResponse>(member);
        var socialsDto = mapper.Map<IEnumerable<SocialResponse>>(member.Socials);
        
        MemberFullInfoResponse memberFullInfoDto = new()
        {
            Id = memberDto.Id,
            PhotoUrl = memberDto.PhotoUrl,
            FullName = memberDto.FullName,
            Description = memberDto.Description,
            Role = memberDto.Role,
            Socials = socialsDto
        };
        
        return memberFullInfoDto;
    }
}