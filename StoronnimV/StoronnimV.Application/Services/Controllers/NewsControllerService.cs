using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Application.Services.Controllers;

public class NewsControllerService(INewsService newsService, IMapper mapper) : INewsControllerService
{
    private readonly INewsService _newsService = newsService;
    private readonly IMapper _mapper = mapper;
    
    public async Task<IEnumerable<NewsResponse>> GetNewsAsync()
    {
        var sortedNews = await _newsService.GetNewsAsync();
        if (sortedNews is null || !sortedNews.Any())
        {
            return new List<NewsResponse>();
        }
        
        return _mapper.Map<IEnumerable<NewsResponse>>(sortedNews).ToList();
    }

    public async Task<IEnumerable<NewsResponse>> GetNewsForPageAsync(int page)
    {
        var sortedNews = await _newsService.GetNewsForPageAsync(page);
        if (sortedNews is null || !sortedNews.Any())
        {
            return new List<NewsResponse>();
        }
        
        return _mapper.Map<IEnumerable<NewsResponse>>(sortedNews).ToList();
    }
}