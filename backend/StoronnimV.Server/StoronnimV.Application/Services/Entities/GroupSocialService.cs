using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Enums;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Services.Entities;

public class GroupSocialService(
    IGroupSocialRepository groupSocialRepository,
    IMediaStorageService mediaStorageService)
    : IGroupSocialService
{
    public async Task<GroupSocialProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        GroupSocialProjection? groupSocialProjection = await groupSocialRepository.GetByIdAsNoTrackingAsync(id, ct);

        if (groupSocialProjection is null)
        {
            throw new EntityNotFoundException($"Group social with {nameof(id)}: {id} was not found");
        }

        return groupSocialProjection;
    }

    public async Task<IEnumerable<GroupSocialProjection>> GetAllAsync(CancellationToken ct)
    {
        var groupSocials = await groupSocialRepository.GetAllAsNoTrackingAsync(ct);

        return groupSocials ?? new List<GroupSocialProjection>();
    }

    public async Task AddGroupSocialAsync(GroupSocialAdditionRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse(request.Name, out SocialType name))
        {
            throw new ArgumentException($"Social type: {request.Name} is not valid.");
        }

        GroupSocial groupSocial = new()
        {
            PhotoUrl = string.Empty,
            Name = name,
            LinkUrl = request.LinkUrl
        };
        
        await mediaStorageService.CreateAsync(
            request.Photo,
            MediaKind.Photo,
            "group-social",
            photoUrl =>
            {
                groupSocial.PhotoUrl = photoUrl;
                return groupSocialRepository.AddAsync(groupSocial, ct);
            },
            ct);
    }

    public async Task DeleteGroupSocialAsync(long id, CancellationToken ct)
    {
        GroupSocial? groupSocial = await groupSocialRepository.GetByIdAsync(id, ct);

        if (groupSocial is null)
        {
            throw new EntityNotFoundException($"Group social with {nameof(id)}: {id} was not found");
        }
        
        await mediaStorageService.DeleteAsync(
            MediaKind.Photo,
            groupSocial.PhotoUrl,
            () => groupSocialRepository.DeleteAsync(groupSocial, ct),
            ct);
    }

    public async Task UpdateGroupSocialAsync(GroupSocialEditRequest request, CancellationToken ct)
    {
        GroupSocial? groupSocial = await groupSocialRepository.GetByIdAsync(request.Id, ct);

        if (groupSocial is null)
        {
            throw new EntityNotFoundException($"Group social with {nameof(request.Id)}: {request.Id} was not found");
        }

        await groupSocialRepository.UpdateAsync(groupSocial, () =>
        {
            groupSocial.LinkUrl = request.LinkUrl;
        }, ct);
    }

    public async Task UpdateGroupSocialPhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        GroupSocial? groupSocial = await groupSocialRepository.GetByIdAsync(request.Id, ct);

        if (groupSocial is null)
        {
            throw new EntityNotFoundException($"Group social with {nameof(request.Id)}: {request.Id} was not found");
        }

        await mediaStorageService.ReplaceAsync(
            request.Photo,
            MediaKind.Photo,
            "group-social",
            groupSocial.PhotoUrl,
            photoUrl => groupSocialRepository.UpdateAsync(
                groupSocial,
                () => groupSocial.PhotoUrl = photoUrl,
                ct),
            ct);
    }
}
