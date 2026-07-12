using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Shared;

namespace StoronnimV.Domain.Projections;

public class GroupSocialProjection : BaseProjection
{
    public required string PhotoUrl { get; init; }
    public required SocialType Name { get; init; }
    public required string LinkUrl { get; init; }
}