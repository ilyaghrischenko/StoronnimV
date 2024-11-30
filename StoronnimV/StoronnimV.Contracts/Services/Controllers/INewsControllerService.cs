using StoronnimV.Contracts.Services.Controllers.Shared;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Contracts.Services.Controllers;

public interface INewsControllerService : IPaginationControllerService<NewsShortResponse>
{
    Task<NewsResponse> GetItemByIdAsync(long id);
    Task<IEnumerable<NewsResponse>> GetAllAsync();
}