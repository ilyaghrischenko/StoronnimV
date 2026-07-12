using Microsoft.AspNetCore.Http;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

/// <summary>
/// DTO для запроса добавления новости
/// </summary>
public class NewsItemAdditionRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public IFormFile? Photo { get; init; }
    public long? VideoId { get; init; }
    public required string Priority { get; init; }
    public string? Date { get; init; }
}