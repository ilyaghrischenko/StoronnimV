namespace StoronnimV.Contracts.Services.Entities.Shared;

public interface IPaginationService
{
    Task<IEnumerable<object>> GetForPageAsync(int page);
}