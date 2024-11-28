using Microsoft.AspNetCore.Mvc;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController(INewsControllerService newsControllerService) : ControllerBase
    {
        private readonly INewsControllerService _newsControllerService = newsControllerService;
        
        [HttpGet("news")]
        public async Task<ActionResult<IEnumerable<NewsResponse>?>> GetNews()
        {
            var news = await _newsControllerService.GetNewsAsync();
            return Ok(news);
        }
    }
}
