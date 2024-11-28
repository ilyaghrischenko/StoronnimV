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
            .ThenBy(news => news.GetPropertyValue("Date"))
            .ToList();
    }
}