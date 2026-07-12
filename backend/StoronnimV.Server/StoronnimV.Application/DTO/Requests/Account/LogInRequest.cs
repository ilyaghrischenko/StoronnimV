namespace StoronnimV.Application.DTO.Requests.Account;

public class LogInRequest
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}