using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{ 
    public class TokenService(IConfiguration config): ITokenService
    {
        public string CreateToken(AppUser user)
        {
            var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access tokenKey from appsettings");
            if (tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            // Add claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName)
            };

            // Create credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Describe how the token will look
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            // Create token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return the written token
            return tokenHandler.WriteToken(token);
        }
    }
}
