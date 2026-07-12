using Microsoft.AspNetCore.Http;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

/// <summary>
/// DTO для запроса добавления участника
/// </summary>
public class MemberAdditionRequest
{
    public required IFormFile PhotoUrl { get; init; }
    public required string FullName { get; init; }
    public required string Description { get; init; }
    public required string Role { get; init; }
}