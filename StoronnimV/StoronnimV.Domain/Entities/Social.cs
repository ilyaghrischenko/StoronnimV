using StoronnimV.Domain.Enums;

namespace StoronnimV.Domain.Entities;

/// <summary>
/// Сущность для хранения информации о социальной сети
/// </summary>
public class Social : BaseEntity
{
    public Member Member { get; set; } = null!;
    public SocialType Type { get; set; } = SocialType.Other;
    public string Url { get; set; } = string.Empty;

    public Social() {}
    public Social(Member member, SocialType type, string url)
    {
        Member = member;
        Type = type;
        Url = url;
    }
}