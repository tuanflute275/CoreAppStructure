using CoreAppStructure.Core.Configurations;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;

namespace CoreAppStructure.Core.Helpers
{
    public class JwtHelper
    {
       /* private readonly string _secretKey;

        public JwtHelper(IOptions<AppSettings> settings)
        {
            _secretKey = settings.Value.SecretKey;
        }

        public string GenerateToken(string userId)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, userId)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/
    }
}
