using StoronnimV.Application.Exceptions;
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
        var member = await memberRepository.GetByIdAsNoTrackingAsync(id)
            ?? throw new EntityNotFoundException($"Member with id: {id} was not found");

        return member;
    }
}