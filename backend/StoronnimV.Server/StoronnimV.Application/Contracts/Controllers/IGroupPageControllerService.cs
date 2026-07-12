using StoronnimV.Application.DTO.Responses.GroupPage;

namespace StoronnimV.Application.Contracts.Controllers;

public interface IGroupPageControllerService
{
    public Task<GroupPageFullInfoResponse> GetGroupPageInfoAsync(CancellationToken ct);
    public Task<MemberFullInfoResponse> GetMemberAsync(long memberId, CancellationToken ct);
}