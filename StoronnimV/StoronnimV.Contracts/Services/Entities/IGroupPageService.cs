using StoronnimV.Contracts.Services.Entities.Shared;

namespace StoronnimV.Contracts.Services.Entities;

public interface IGroupPageService : IReceivableService
{
    public Task<object> GetFirstGroupPageAsync();
}