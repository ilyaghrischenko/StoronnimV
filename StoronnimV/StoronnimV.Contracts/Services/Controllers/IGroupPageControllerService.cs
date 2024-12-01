using StoronnimV.DTO.Responses.GroupPage;

namespace StoronnimV.Contracts.Services.Controllers;

public interface IGroupPageControllerService
{
    public Task<GroupPageFullInfoResponse> GetGroupPageInfoAsync();
    public Task<MemberFullInfoResponse> GetMemberInfoAsync(long memberId);
}