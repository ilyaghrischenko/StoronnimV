using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Application.Services.Entities;

public class MemberService(IMemberRepository memberRepository) : IMemberService
{
    public async Task<IEnumerable<object>> GetAllAsync()
    {
        var members = await memberRepository.GetAllAsync();
        return members ?? new List<object>();
    }

    public async Task<object> GetItemByIdAsync(long id)
    {
        var member = await memberRepository.GetByIdAsNoTrackingAsync(id);

        //TODO: Сделать ексепшн для ситуации, когда нет сущности по такому айди (MemberService)
        return member ?? throw new Exception();
    }
}