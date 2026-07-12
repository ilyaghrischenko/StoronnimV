using Microsoft.AspNetCore.Http;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

public class GroupSocialAdditionRequest
{
    public required IFormFile Photo { get; init; }
    public required string Name { get; init; }
    public required string LinkUrl { get; init; }
}