using StoronnimV.Application.Contracts.Entities.Shared;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Contracts.Entities;

public interface IGroupSocialService
    : IGetByIdService<GroupSocialProjection>, IGetAllService<GroupSocialProjection>
{
    Task AddGroupSocialAsync(GroupSocialAdditionRequest request, CancellationToken ct);
    Task DeleteGroupSocialAsync(long id, CancellationToken ct);
    Task UpdateGroupSocialAsync(GroupSocialEditRequest request, CancellationToken ct);
}