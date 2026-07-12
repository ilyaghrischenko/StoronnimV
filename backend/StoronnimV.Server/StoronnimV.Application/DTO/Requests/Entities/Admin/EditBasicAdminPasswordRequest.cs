namespace StoronnimV.Application.DTO.Requests.Entities.Admin;

public class EditBasicAdminPasswordRequest
{
    public required string OldPassword { get; init; }
    public required string NewPassword { get; init; }
}