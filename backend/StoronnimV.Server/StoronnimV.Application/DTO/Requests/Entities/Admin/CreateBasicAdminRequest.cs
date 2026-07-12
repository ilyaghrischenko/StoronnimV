namespace StoronnimV.Application.DTO.Requests.Entities.Admin;

public class CreateBasicAdminRequest
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}