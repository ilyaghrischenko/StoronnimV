using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.HomePage;

public class NewsHomeResponse : BaseResponseDto
{
    public string? Photo { get; init; }
    public required string Title { get; init; }
}
