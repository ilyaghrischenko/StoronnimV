using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.Admin;

public class BasicAdminProjection : BaseProjection
{
    public required string Login { get; init; }
}