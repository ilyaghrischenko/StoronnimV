using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Application.Services.Controllers;

public class NewsControllerService(
    INewsService newsService,
    IMapper mapper) : INewsControllerService
{
    private readonly INewsService _newsService = newsService;
    private readonly IMapper _mapper = mapper;

    public async Task<NewsResponse> GetNewsItemByIdAsync(long id)
    {
        var newsItem = await _newsService.GetNewsItemByIdAsync(id);

        var newsItemDto = _mapper.Map<NewsResponse>(newsItem);
        return newsItemDto;
    }

    public async Task<IEnumerable<NewsResponse>> GetNewsAsync()
    {
        var sortedNews = await _newsService.GetNewsAsync();

        var newsDto = _mapper.Map<IEnumerable<NewsResponse>>(sortedNews).ToList();
        return newsDto;
    }

    public async Task<IEnumerable<NewsShortResponse>> GetNewsForPageAsync(int page)
    {
        var sortedNews = await _newsService.GetNewsForPageAsync(page);
        
        var newsDto = _mapper.Map<IEnumerable<NewsShortResponse>>(sortedNews).ToList();
        return newsDto;
    }
}