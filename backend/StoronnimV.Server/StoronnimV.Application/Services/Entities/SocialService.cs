using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.Exceptions;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Services.Entities;

/// <summary>
/// Сервис для проверки полученных данных, полученых с репозитория
/// </summary>
/// <param name="socialRepository"></param>
public class SocialService(
    ISocialRepository socialRepository,
    IMemberRepository memberRepository) : ISocialService
{
    public async Task<SocialProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        SocialProjection social = await socialRepository.GetByIdAsNoTrackingAsync(id, ct)
                                       ?? throw new EntityNotFoundException($"Social with {nameof(id)}: {id} was not found");
        
        return social;
    }

    /// <summary>
    /// Social addition to database
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    public async Task AddSocialAsync(SocialAdditionRequest request, CancellationToken ct)
    {
        Member? member = await memberRepository.GetByIdAsync(request.MemberId, ct);
        
        if (member is null)
        {
            throw new EntityNotFoundException($"Member with {nameof(request.MemberId)}: {request.MemberId} was not found");
        }
        
        Social social = new()
        {
            Member = member,
            Url = request.Url,
            Type = Enum.Parse<SocialType>(request.Type)
        };
        
        await socialRepository.AddAsync(social, ct);
    }
    
    /// <summary>
    /// Social deletion from database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteSocialAsync(long id, CancellationToken ct)
    {
        Social? social = await socialRepository.GetByIdAsync(id, ct);

        if (social is null)
        {
            throw new EntityNotFoundException($"Social with {nameof(id)}: {id} was not found");
        }

        await socialRepository.DeleteAsync(social, ct);
    }
    
    public async Task UpdateSocialAsync(SocialEditRequest request, CancellationToken ct)
    {
        Social? social = await socialRepository.GetByIdAsync(request.Id, ct);
        
        if (social is null)
        {
            throw new EntityNotFoundException($"Social with {nameof(request.Id)}: {request.Id} was not found");
        }
        
        await socialRepository.UpdateAsync(social, () =>
        {
            social.Url = request.Url;
            social.Type = Enum.Parse<SocialType>(request.Type);
        }, ct);
    }
}