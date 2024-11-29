using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Contracts.Services.Controllers;

public interface INewsControllerService
{
    Task<NewsResponse> GetNewsItemByIdAsync(long id);
    Task<IEnumerable<NewsResponse>> GetNewsAsync();
    Task<IEnumerable<NewsShortResponse>> GetNewsForPageAsync(int page);
}