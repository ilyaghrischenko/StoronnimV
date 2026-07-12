using StoronnimV.Domain.Entities.Shared;
using StoronnimV.Domain.Enums;

namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность для хранения информации о социальной сети
/// </summary>
public class Social : BaseEntity
{
    public required Member Member { get; set; }
    public required string Url { get; set; } = string.Empty;
    public required SocialType Type { get; set; } = SocialType.Other;

    public Social() {}
}