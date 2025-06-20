using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthAPI.Services
{
    public class TokenService
    {
        private static List<string> invalidatedTokens = new List<string>();

        public static string Secret = "este_es_un_secret_clave_para_demo";

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username!)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void InvalidateToken(string token)
        {
            invalidatedTokens.Add(token);
        }

        public bool IsTokenInvalidated(string token)
        {
            return invalidatedTokens.Contains(token);
        }
    }
}
