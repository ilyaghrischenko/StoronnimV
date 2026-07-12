using StoronnimV.Application.Contracts.Controllers.Shared;
using StoronnimV.Application.DTO.Responses.NewsPage;

namespace StoronnimV.Application.Contracts.Controllers;

public interface INewsControllerService
    : IGetByIdControllerService<NewsResponse>, IPaginationControllerService<NewsShortResponse>
{
    
}