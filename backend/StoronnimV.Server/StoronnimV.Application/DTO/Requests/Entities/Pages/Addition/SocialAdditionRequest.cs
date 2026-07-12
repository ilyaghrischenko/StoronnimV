namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

/// <summary>
/// DTO для запроса добавления социальной сети участника
/// </summary>
public class SocialAdditionRequest
{
    public required long MemberId { get; init; }
    public required string Url { get; init; }
    public required string Type { get; init; }
}