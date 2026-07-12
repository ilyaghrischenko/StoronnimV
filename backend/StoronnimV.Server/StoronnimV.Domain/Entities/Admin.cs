using StoronnimV.Domain.Entities.Shared;
using StoronnimV.Domain.Enums;

namespace StoronnimV.Domain.Entities;

public class Admin : BaseEntity
{
    public required string Login { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
    
    public AdminType Type { get; set; } = AdminType.Basic;
    
    public Admin() {}
}
