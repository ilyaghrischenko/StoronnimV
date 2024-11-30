using Microsoft.AspNetCore.Mvc;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Api.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController(INewsControllerService newsControllerService) : ControllerBase
    {
        private readonly INewsControllerService _newsControllerService = newsControllerService;

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsResponse>> GetNewsItem([FromRoute] long id)
        {
            var newsItem = await _newsControllerService.GetItemByIdAsync(id);
            return Ok(newsItem);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsResponse>?>> GetNews()
        {
            var news = await _newsControllerService.GetAllAsync();
            return Ok(news);
        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<NewsShortResponse>?>> GetNewsForPage([FromRoute] int page)
        {
            var news = await _newsControllerService.GetForPageAsync(page);
            return Ok(news);
        }
    }
}
