using System.ComponentModel.DataAnnotations;

namespace StoronnimV.Application.Options;

public class CookieSettings
{
    [Required]
    public bool HttpOnly { get; set; }
    
    [Required]
    public bool Secure { get; set; }
    
    [Required]
    public required string SameSite { get; set; }
    
    [Required]
    public int ExpiresInHours { get; set; }
    
    public string? Domain { get; set; }
}