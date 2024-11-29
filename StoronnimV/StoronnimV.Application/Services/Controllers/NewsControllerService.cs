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