namespace StoronnimV.Contracts.Repositories.Shared;

public interface IPaginationRepository
{
    Task<IEnumerable<object>?> GetForPageAsync(int page, int pageSize = 10);
}