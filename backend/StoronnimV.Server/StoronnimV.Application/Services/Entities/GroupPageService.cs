using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Enums;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Services.Entities;

/// <summary>
/// Сервис для проверки полученных данных, полученых с репозитория
/// </summary>
/// <param name="groupPageRepository"></param>
public class GroupPageService(
    IGroupPageRepository groupPageRepository,
    IMediaStorageService mediaStorageService) : IGroupPageService
{
    public async Task<GroupPageProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        GroupPageProjection groupPage = await groupPageRepository.GetByIdAsNoTrackingAsync(id, ct)
                                        ?? throw new EntityNotFoundException(
                                            $"GroupPage with {nameof(id)}: {id} was not found");

        return groupPage;
    }

    public async Task<IEnumerable<GroupPageProjection>> GetAllAsync(CancellationToken ct)
    {
        var groupPages = await groupPageRepository.GetAllAsNoTrackingAsync(ct);

        return groupPages ?? new List<GroupPageProjection>();
    }

    public async Task<GroupPageProjection> GetFirstGroupPageAsync(CancellationToken ct)
    {
        GroupPageProjection groupPage = await groupPageRepository.GetFirstGroupPageAsync(ct)
                                        ?? throw new EntityNotFoundException("GroupPage was not found");

        return groupPage;
    }

    /// <summary>
    /// GroupPage addition to database
    /// </summary>
    /// <param name="request">GroupPageAdditionRequest</param>
    /// <param name="ct">CancellationToken</param>
    public async Task AddGroupPageAsync(GroupPageAdditionRequest request, CancellationToken ct)
    {
        var existingGroupPages = await groupPageRepository.GetAllAsNoTrackingAsync(ct);

        if (existingGroupPages?.Any() == true)
        {
            throw new ArgumentException("GroupPage already exists");
        }

        GroupPage groupPage = new()
        {
            PhotoUrl = string.Empty,
            Description = request.Description,
        };

        await mediaStorageService.CreateAsync(
            request.PhotoUrl,
            MediaKind.Photo,
            "group-page",
            photoUrl =>
            {
                groupPage.PhotoUrl = photoUrl;
                return groupPageRepository.AddAsync(groupPage, ct);
            },
            ct);
    }

    /// <summary>
    /// GroupPage deletion from database
    /// </summary>
    /// <param name="id">long</param>
    /// <param name="ct">CancellationToken</param>
    /// <exception cref="EntityNotFoundException">EntityNotFoundException</exception>
    public async Task DeleteGroupPageAsync(long id, CancellationToken ct)
    {
        GroupPage? groupPage = await groupPageRepository.GetByIdAsync(id, ct);

        if (groupPage is null)
        {
            throw new EntityNotFoundException($"Group page with {nameof(id)}: {id} was not found");
        }

        await mediaStorageService.DeleteAsync(
            MediaKind.Photo,
            groupPage.PhotoUrl,
            () => groupPageRepository.DeleteAsync(groupPage, ct),
            ct);
    }

    public async Task UpdateGroupPageAsync(GroupPageEditRequest request, CancellationToken ct)
    {
        GroupPage? groupPage = await groupPageRepository.GetByIdAsync(request.Id, ct);

        if (groupPage is null)
        {
            throw new EntityNotFoundException($"GroupPage with {nameof(request.Id)}: {request.Id} was not found");
        }

        if (string.IsNullOrEmpty(request.Description))
        {
            throw new ArgumentException("Description is required");
        }

        await groupPageRepository.UpdateAsync(groupPage, () => groupPage.Description = request.Description, ct);
    }

    public async Task UpdateGroupPagePhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        GroupPage? groupPage = await groupPageRepository.GetByIdAsync(request.Id, ct);

        if (groupPage is null)
        {
            throw new EntityNotFoundException($"GroupPage with {nameof(request.Id)}: {request.Id} was not found");
        }

        if (request.Photo is null)
        {
            throw new ArgumentException("Photo is required");
        }

        await mediaStorageService.ReplaceAsync(
            request.Photo,
            MediaKind.Photo,
            "group-page",
            groupPage.PhotoUrl,
            photoUrl => groupPageRepository.UpdateAsync(groupPage, () => groupPage.PhotoUrl = photoUrl, ct),
            ct);
    }
}
