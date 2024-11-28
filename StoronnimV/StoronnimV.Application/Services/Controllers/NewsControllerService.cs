using StoronnimV.Application.Extensions;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Application.Services.Controllers;

public class NewsControllerService(INewsService newsService) : INewsControllerService
{
    private readonly INewsService _newsService = newsService;
    
    public async Task<IEnumerable<NewsResponse>> GetNewsAsync()
    {
        var sortedNews = await _newsService.GetNewsAsync();
        if (sortedNews is null || !sortedNews.Any())
        {
            return new List<NewsResponse>();
        }
        
        return sortedNews
            .Select(news => new NewsResponse(
                (long)news.GetPropertyValue("Id"),
                (string)news.GetPropertyValue("Photo"),
                (string)news.GetPropertyValue("Title"),
                (string)news.GetPropertyValue("Description"),
                (string)news.GetPropertyValue("Priority"),
                (string)news.GetPropertyValue("Date")
            ))
            .ToList();
    }

    public async Task<IEnumerable<NewsResponse>> GetNewsForPageAsync(int page)
    {
        var sortedNews = await _newsService.GetNewsForPageAsync(page);
        if (sortedNews is null || !sortedNews.Any())
        {
            return new List<NewsResponse>();
        }
        
        return sortedNews
            .Select(news => new NewsResponse(
                (long)news.GetPropertyValue("Id"),
                (string)news.GetPropertyValue("Photo"),
                (string)news.GetPropertyValue("Title"),
                (string)news.GetPropertyValue("Description"),
                (string)news.GetPropertyValue("Priority"),
                (string)news.GetPropertyValue("Date")
            ))
            .ToList();
    }
}