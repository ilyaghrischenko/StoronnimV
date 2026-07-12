using StoronnimV.Domain.Entities.Shared;
using StoronnimV.Domain.Enums;

namespace StoronnimV.Domain.Entities;

public class GroupSocial : BaseEntity
{
    public required string PhotoUrl { get; set; } = string.Empty;
    public required SocialType Name { get; set; } = SocialType.Other;
    public required string LinkUrl { get; set; } = string.Empty;
    
    public GroupSocial() {}
}