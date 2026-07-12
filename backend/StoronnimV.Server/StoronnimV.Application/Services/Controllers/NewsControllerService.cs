using AutoMapper;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Responses.NewsPage;
using StoronnimV.Application.DTO.Responses.Shared;
using StoronnimV.Application.Models;
using StoronnimV.Domain.Projections.News;

namespace StoronnimV.Application.Services.Controllers;

/// <summary>
/// Сервис для маппинга данных с бд и возвращения контроллеру
/// </summary>
/// <param name="newsService"></param>
/// <param name="mapper"></param>
public class NewsControllerService(
    INewsService newsService,
    IMapper mapper) : INewsControllerService
{
    public async Task<NewsResponse> GetItemByIdAsync(long id, CancellationToken ct)
    {
        NewsFullProjection newsItem = await newsService.GetItemByIdAsync(id, ct);

        var newsItemDto = mapper.Map<NewsResponse>(newsItem);
        
        return newsItemDto;
    }

    public async Task<PaginationResponse<NewsShortResponse>> GetForPageAsync(int page, int pageSize, CancellationToken ct, params object[] args)
    {
        PaginationResult<NewsPaginationProjection> paginationResult = await newsService.GetForPageAsync(page, pageSize, ct);
        
        var newsDto = mapper.Map<IEnumerable<NewsShortResponse>>(paginationResult.Items);
        
        var response = new PaginationResponse<NewsShortResponse>
        {
            CurrentPage = paginationResult.CurrentPage,
            TotalPages = paginationResult.TotalPages,
            TotalItems = paginationResult.TotalItems,
            Items = newsDto
        };
        
        return response;
    }
}