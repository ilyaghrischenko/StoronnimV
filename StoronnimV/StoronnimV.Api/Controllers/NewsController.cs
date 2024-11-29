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
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsResponse>?>> GetNews()
        {
            var news = await _newsControllerService.GetNewsAsync();
            return Ok(news);
        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<NewsShortResponse>?>> GetNewsForPage([FromRoute] int page)
        {
            var news = await _newsControllerService.GetNewsForPageAsync(page);
            return Ok(news);
        }
    }
}
