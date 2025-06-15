using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core;
using Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Extensions;

public static class UserExtension
{
    public static string GenerateJwtToken(this User user, AuthTokenOptions options)
    {
        //TODO: get key from a decent place
        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key).ToArray()),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email),
            new (JwtRegisteredClaimNames.Name, user.Username),
            new ("Role", user.Role.ToString())
        };
        
        //TODO: Same with issuer and audience
        var token = new JwtSecurityToken(options.Issuer, 
                                        options.Audience, 
                                                claims, 
                                        DateTime.Now, 
                                        DateTime.Now.AddMinutes(options.TokenDuration), 
                                        creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}