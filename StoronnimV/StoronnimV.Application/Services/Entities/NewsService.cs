using StoronnimV.Application.Extensions;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

public class NewsService(INewsRepository newsRepository) : INewsService
{
    private readonly INewsRepository _newsRepository = newsRepository;
    
    public async Task<IEnumerable<object>> GetNewsAsync()
    {
        var allNews = await _newsRepository.GetAllAsync();
        if (allNews is null)
        {
            return new List<object>();
        }
        
        return allNews
            .OrderBy(news => news.GetPropertyValue("Priority"))
            .ThenByDescending(news => news.GetPropertyValue("Date"))
            .ToList();
    }

    public async Task<IEnumerable<object>> GetNewsForPageAsync(int page)
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