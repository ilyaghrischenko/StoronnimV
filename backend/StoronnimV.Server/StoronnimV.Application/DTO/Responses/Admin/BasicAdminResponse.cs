using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.DTO.Responses.Admin;

public class BasicAdminResponse : BaseResponseDto
{
    public required string Login { get; init; }
}