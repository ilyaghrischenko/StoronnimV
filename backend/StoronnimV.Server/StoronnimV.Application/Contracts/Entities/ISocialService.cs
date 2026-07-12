using StoronnimV.Application.Contracts.Entities.Shared;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Contracts.Entities;

public interface ISocialService : IGetByIdService<SocialProjection>
{
    Task AddSocialAsync(SocialAdditionRequest request, CancellationToken ct);
    Task DeleteSocialAsync(long id, CancellationToken ct);
    Task UpdateSocialAsync(SocialEditRequest request, CancellationToken ct);
}