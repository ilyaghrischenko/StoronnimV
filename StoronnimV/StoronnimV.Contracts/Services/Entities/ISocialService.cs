using StoronnimV.Contracts.Services.Entities.Shared;

namespace StoronnimV.Contracts.Services.Entities;

public interface ISocialService : IReceivableService
{
    public Task<object> GetAllForMemberAsync(long memberId);
}