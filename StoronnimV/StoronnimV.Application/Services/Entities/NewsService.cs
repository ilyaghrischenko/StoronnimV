using StoronnimV.Application.Extensions;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

public class NewsService(INewsRepository newsRepository) : INewsService
{
    private readonly INewsRepository _newsRepository = newsRepository;

    public async Task<object> GetItemByIdAsync(long id)
    {
        var newsItem = await _newsRepository.GetByIdAsNoTrackingAsync(id)
            ?? throw new NullReferenceException($"News with id: {id} was not found");
        
        return newsItem;
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        var allNews = await _newsRepository.GetAllAsync();
        if (allNews is null || !allNews.Any())
        {
            return new List<object>();
        }
        
        return allNews
            .OrderBy(news => news.GetPropertyValue("Priority"))
            .ThenByDescending(news => news.GetPropertyValue("Date"))
            .ToList();
    }

    public async Task<IEnumerable<object>> GetForPageAsync(int page)
    {
        var allNews = await _newsRepository.GetForPageAsync(page);
        if (allNews is null)
        {
            return new List<object>();
        }
        
        return allNews
            .OrderBy(news => news.GetPropertyValue("Priority"))
            .ToList();
    }
}