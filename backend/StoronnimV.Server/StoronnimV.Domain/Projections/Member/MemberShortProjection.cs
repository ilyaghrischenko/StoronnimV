using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.Member;

public class MemberShortProjection : BaseProjection
{
    public required string PhotoUrl { get; init; }
    public required string FullName { get; init; }
    public required string Role { get; init; }
}