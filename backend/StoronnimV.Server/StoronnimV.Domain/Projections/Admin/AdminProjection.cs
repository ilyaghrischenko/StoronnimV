using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.Admin;

public class AdminProjection : BaseProjection
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}