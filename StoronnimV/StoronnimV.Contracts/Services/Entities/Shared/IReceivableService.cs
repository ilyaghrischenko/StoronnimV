namespace StoronnimV.Contracts.Services.Entities.Shared;

public interface IReceivableService
{
    Task<object> GetItemByIdAsync(long id);
    Task<IEnumerable<object>> GetAllAsync();
}