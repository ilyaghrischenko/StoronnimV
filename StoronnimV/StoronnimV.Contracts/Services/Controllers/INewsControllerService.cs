using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Contracts.Services.Controllers;

public interface INewsControllerService
{
    Task<IEnumerable<NewsResponse>> GetNewsAsync();
}