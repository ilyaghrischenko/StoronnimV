namespace StoronnimV.Contracts.Services.Entities;

public interface INewsService
{
    Task<IEnumerable<object>> GetNewsAsync();
    Task<IEnumerable<object>> GetNewsForPageAsync(int page);
}