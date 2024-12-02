using StoronnimV.Application.Exceptions;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

/// <summary>
/// Сервис для проверки полученных данных, полученых с репозитория
/// </summary>
/// <param name="socialRepository"></param>
public class SocialService(ISocialRepository socialRepository) : ISocialService
{
    public async Task<object> GetItemByIdAsync(long id)
    {
        var social = await socialRepository.GetByIdAsNoTrackingAsync(id)
            ?? throw new EntityNotFoundException($"Social with id: {id} was not found");
        
        return social;
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        var socials = await socialRepository.GetAllAsync();
        
        return socials ?? new List<object>();
    }
    
    public async Task<object> GetAllForMemberAsync(long memberId)
    {
        var socials = await socialRepository.GetAllForMemberAsync(memberId)
            ?? throw new EntityNotFoundException($"Socials with member id: {memberId} was not found");
        
        return socials;
    }
}