using Microsoft.AspNetCore.Http;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

/// <summary>
/// DTO для запроса добавления платформы музыки
/// </summary>
public class MusicPlatformAdditionRequest
{
    public required IFormFile BgImageUrl { get; init; }
    public required string PlatformUrl { get; init; }
}