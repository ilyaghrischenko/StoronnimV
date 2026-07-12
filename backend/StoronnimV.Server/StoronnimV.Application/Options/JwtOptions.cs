using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace StoronnimV.Application.Options;

public class JwtOptions
{
    [Required]
    public required string ISSUER {get; set;}
    
    [Required]
    public required string AUDIENCE {get; set;}
    
    [Required]
    public required string KEY {get; set;}
    
    [Required]
    public required int LIFETIME {get; set;}

    public SymmetricSecurityKey GetKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}