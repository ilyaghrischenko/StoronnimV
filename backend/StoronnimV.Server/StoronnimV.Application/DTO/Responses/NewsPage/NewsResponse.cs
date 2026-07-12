using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.NewsPage;

public class NewsResponse : BaseResponseDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Priority { get; init; }
    public required string Date { get; init; }

    public string? Photo { get; init; }
    public string? Video { get; init; }
}