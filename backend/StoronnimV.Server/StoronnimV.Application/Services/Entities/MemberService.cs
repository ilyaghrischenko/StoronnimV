using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Enums;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections.Member;

namespace StoronnimV.Application.Services.Entities;

/// <summary>
/// Сервис для проверки полученных данных, полученых с репозитория
/// </summary>
/// <param name="memberRepository"></param>
public class MemberService(
    IMemberRepository memberRepository,
    IMediaStorageService mediaStorageService) : IMemberService
{
    public async Task<IEnumerable<MemberShortProjection>> GetAllAsync(CancellationToken ct)
    {
        var members = await memberRepository.GetAllAsNoTrackingAsync(ct);

        return members ?? new List<MemberShortProjection>();
    }

    public async Task<MemberFullProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        MemberFullProjection member = await memberRepository.GetByIdAsNoTrackingAsync(id, ct)
                                      ?? throw new EntityNotFoundException(
                                          $"Member with {nameof(id)}: {id} was not found");

        return member;
    }

    /// <summary>
    /// Member adding to database
    /// </summary>
    /// <param name="request">MemberAdditionRequest</param>
    /// <param name="ct">CancellationToken</param>
    public async Task AddMemberAsync(MemberAdditionRequest request, CancellationToken ct)
    {
        Member member = new()
        {
            PhotoUrl = string.Empty,
            FullName = request.FullName,
            Description = request.Description,
            Role = request.Role
        };

        await mediaStorageService.CreateAsync(
            request.PhotoUrl,
            MediaKind.Photo,
            "member",
            photoUrl =>
            {
                member.PhotoUrl = photoUrl;
                return memberRepository.AddAsync(member, ct);
            },
            ct);
    }

    /// <summary>
    /// Member deleting from database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteMemberAsync(long id, CancellationToken ct)
    {
        Member? member = await memberRepository.GetByIdAsync(id, ct);

        if (member is null)
        {
            throw new EntityNotFoundException($"Member with {nameof(id)}: {id} was not found");
        }

        await mediaStorageService.DeleteAsync(
            MediaKind.Photo,
            member.PhotoUrl,
            () => memberRepository.DeleteAsync(member, ct),
            ct);
    }

    public async Task UpdateMemberAsync(MemberEditRequest request, CancellationToken ct)
    {
        Member? member = await memberRepository.GetByIdAsync(request.Id, ct);

        if (member is null)
        {
            throw new EntityNotFoundException($"Member with {nameof(request.Id)}: {request.Id} was not found");
        }

        await memberRepository.UpdateAsync(member,
            () =>
            {
                member.FullName = request.FullName;
                member.Description = request.Description;
                member.Role = request.Role;
            }, ct);
    }

    public async Task UpdateMemberPhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        Member? member = await memberRepository.GetByIdAsync(request.Id, ct);

        if (member is null)
        {
            throw new EntityNotFoundException($"Member with {nameof(request.Id)}: {request.Id} was not found");
        }

        await mediaStorageService.ReplaceAsync(
            request.Photo,
            MediaKind.Photo,
            "member",
            member.PhotoUrl,
            photoUrl => memberRepository.UpdateAsync(member, () => member.PhotoUrl = photoUrl, ct),
            ct);
    }
}
