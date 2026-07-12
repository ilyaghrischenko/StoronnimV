using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.DTO.Responses.NewsPage;
using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Api.Controllers;

/// <summary>
/// Контроллер для страницы 'Новости', он позволяет доставать нужные данные для отображения
/// </summary>
/// <param name="newsControllerService"></param>
[EnableRateLimiting("UserLimitPerMinute")]
[Route("api/news")]
[ApiController]
public class NewsController(INewsControllerService newsControllerService) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<ActionResult<NewsResponse>> GetNewsItem([FromRoute] long id, CancellationToken ct)
    {
        NewsResponse newsItem = await newsControllerService.GetItemByIdAsync(id, ct);

        return Ok(newsItem);
    }

    [HttpGet("page/{page:int}")]
    public async Task<ActionResult<PaginationResponse<NewsShortResponse>>> GetNewsForPage([FromRoute] int page,
        CancellationToken ct, [FromQuery] int pageSize = 9)
    {
        var newsPaginationResponse = await newsControllerService.GetForPageAsync(page, pageSize, ct);

        return Ok(newsPaginationResponse);
    }
}