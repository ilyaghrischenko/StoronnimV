using StoronnimV.Contracts.Services.Controllers.Shared;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Contracts.Services.Controllers;

public interface INewsControllerService : IReceivableControllerService<NewsResponse>
{
    Task<IEnumerable<NewsShortResponse>> GetForPageAsync(int page);
}