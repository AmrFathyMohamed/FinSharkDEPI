using FinShark.Interfaces;
using FinShark.Models;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
namespace FinShark.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            var signkey = config["JWT:SigningKey"] ?? throw new InvalidOperationException("JWT:SigningKey is not Configured");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signkey));

        }

        public string CreateToken(AppUser user, IList<string> roles)
        {
            var clamis = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email)
            };

            clamis.AddRange(roles.Select(role=> new Claim(ClaimTypes.Role,role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(clamis),
                Expires = DateTime.UtcNow.AddDays(2),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                SigningCredentials = new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature)
            };

            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }
    }
}
