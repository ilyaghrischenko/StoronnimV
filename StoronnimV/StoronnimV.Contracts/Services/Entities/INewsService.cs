namespace StoronnimV.Contracts.Services.Entities;

public interface INewsService
{
    Task<object> GetNewsItemByIdAsync(long id);
    Task<IEnumerable<object>> GetNewsAsync();
    Task<IEnumerable<object>> GetNewsForPageAsync(int page);
}