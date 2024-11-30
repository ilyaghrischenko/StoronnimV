using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

public class SocialService(ISocialRepository socialRepository) : ISocialService
{
    public async Task<object> GetItemByIdAsync(long id)
    {
        var social = await socialRepository.GetByIdAsNoTrackingAsync(id);
        
        //TODO: Сделать ексепшн для ситуации, когда нет сущности по такому айди (SocialService)
        return social ?? throw new Exception();
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        var socials = await socialRepository.GetAllAsync();
        
        return socials ?? new List<object>();
    }
    
    public async Task<object> GetAllForMemberAsync(long memberId)
    {
        var socials = await socialRepository.GetAllForMemberAsync(memberId);
        
        //TODO: Сделать ексепшн для ситуации, когда нет сущности (SocialService)
        return socials ?? throw new Exception();
    }
}