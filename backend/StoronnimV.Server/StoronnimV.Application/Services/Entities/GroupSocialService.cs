using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.Exceptions;
using StoronnimV.Domain.Contracts.AzureBlobStorage;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Services.Entities;

public class GroupSocialService(
    IGroupSocialRepository groupSocialRepository,
    IBlobRepository blobRepository)
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
            PhotoUrl = "default",
            Name = name,
            LinkUrl = request.LinkUrl
        };
        
        await groupSocialRepository.AddAsync(groupSocial, ct);
        
        string groupSocialBlobName = $"group-social-{groupSocial.Id}";

        string extension = Path.GetExtension(request.Photo.FileName);
        
        string groupSocialPhotoUrl = await blobRepository.AddFileAndGetUrlAsync(
            "storonnimv-photo", 
            $"{groupSocialBlobName}{extension}",
            request.Photo.OpenReadStream(),
            ct
        );
        
        await groupSocialRepository.UpdateAsync(groupSocial, () =>
        {
            groupSocial.PhotoUrl = groupSocialPhotoUrl;
        }, ct);
    }

    public async Task DeleteGroupSocialAsync(long id, CancellationToken ct)
    {
        GroupSocial? groupSocial = await groupSocialRepository.GetByIdAsync(id, ct);

        if (groupSocial is null)
        {
            throw new EntityNotFoundException($"Group social with {nameof(id)}: {id} was not found");
        }
        
        await groupSocialRepository.DeleteAsync(groupSocial, ct);
        
        await blobRepository.DeleteAllFilesByNameAsync("storonnimv-photo", $"group-social-{id}", ct);
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
}