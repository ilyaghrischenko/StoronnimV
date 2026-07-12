using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.Exceptions;
using StoronnimV.Domain.Contracts.AzureBlobStorage;
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
    IBlobRepository blobRepository) : IMemberService
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
            PhotoUrl = "default",
            FullName = request.FullName,
            Description = request.Description,
            Role = request.Role
        };

        await memberRepository.AddAsync(member, ct);

        string memberPhotoBlobName = $"member-{member.Id}";
        string extension = Path.GetExtension(request.PhotoUrl.FileName);
        string memberPhotoUrl = await blobRepository
            .AddFileAndGetUrlAsync("storonnimv-photo", $"{memberPhotoBlobName}{extension}",
                request.PhotoUrl.OpenReadStream(), ct);

        await memberRepository.UpdateAsync(member, () => member.PhotoUrl = memberPhotoUrl, ct);
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

        await memberRepository.DeleteAsync(member, ct);

        await blobRepository.DeleteAllFilesByNameAsync("storonnimv-photo", $"member-{id}", ct);
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

        string memberPhotoBlobName = $"member-{member.Id}";

        await blobRepository.DeleteAllFilesByNameAsync("storonnimv-photo", memberPhotoBlobName, ct);

        string extension = Path.GetExtension(request.Photo.FileName);
        string memberPhotoUrl = await blobRepository.AddFileAndGetUrlAsync
            ("storonnimv-photo", $"{memberPhotoBlobName}{extension}", request.Photo.OpenReadStream(), ct);

        await memberRepository.UpdateAsync(member, () => member.PhotoUrl = memberPhotoUrl, ct);
    }
}