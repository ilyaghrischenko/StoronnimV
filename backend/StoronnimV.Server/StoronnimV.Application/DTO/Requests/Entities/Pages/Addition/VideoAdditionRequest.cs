using Microsoft.AspNetCore.Http;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

/// <summary>
/// DTO для запроса добавления видео
/// </summary>
public class VideoAdditionRequest
{
    public required IFormFile Url { get; init; }
    public required string Title { get; init; }
    public required string Type { get; init; }
}