using Microsoft.AspNetCore.Mvc;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Api.Controllers
{
    /// <summary>
    /// Контроллер для страницы 'Новости', он позволяет доставать нужные данные для отображения
    /// </summary>
    /// <param name="newsControllerService"></param>
    [Route("api/news")]
    [ApiController]
    public class NewsController(INewsControllerService newsControllerService) : ControllerBase
    {
        private readonly INewsControllerService _newsControllerService = newsControllerService;

        [HttpGet("{id:long}")]
        public async Task<ActionResult<NewsResponse>> GetNewsItem([FromRoute] long id)
        {
            var newsItem = await _newsControllerService.GetItemByIdAsync(id);
            return Ok(newsItem);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsResponse>>> GetNews()
        {
            var news = await _newsControllerService.GetAllAsync();
            return Ok(news);
        }

        [HttpGet("page/{page:int}")]
        public async Task<ActionResult<IEnumerable<NewsShortResponse>>> GetNewsForPage([FromRoute] int page)
        {
            var news = await _newsControllerService.GetForPageAsync(page);
            return Ok(news);
        }
    }
}
