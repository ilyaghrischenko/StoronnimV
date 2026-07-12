using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections.Member;

public class MemberFullProjection : BaseProjection
{
    public required string PhotoUrl { get; init; }
    public required string FullName { get; init; }
    public required string Description { get; init; }
    public required string Role { get; init; }
    public required IEnumerable<SocialProjection> Socials { get; init; }
}