using Microsoft.AspNetCore.Http;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

/// <summary>
/// DTO для запроса добавления страницы о группе
/// </summary>
public class GroupPageAdditionRequest
{
    public required IFormFile PhotoUrl { get; init; }
    public required string Description { get; init; }
}