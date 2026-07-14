using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
    [BindRequired]
    public required DateOnly Date { get; init; }
}
