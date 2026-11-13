using FinShark.Application.Interfaces;
using FinShark.Domain.Entities.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
namespace FinShark.Infrastructure.Services
{
    // Identity: الخدمة اللي بتعمل الـ JWT بعد Register / Login.
    // متسجّلة في Program.cs كـ AddScoped<ITokenService, TokenService>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        // المفتاح اللي بنوقّع بيه الـ token — نفس المفتاح اللي AddJwtBearer بيتحقق بيه في Program.cs
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // المفتاح من appsettings.json → JWT:SigningKey (لازم أطول من 512 bit عشان HmacSha512 وإلا CreateToken بيقع)
            var signkey = config["JWT:SigningKey"] ?? throw new InvalidOperationException("JWT:SigningKey is not Configured");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signkey));

        }

        public string CreateToken(AppUser user, IList<string> roles)
        {
            // الـ Claims = المعلومات اللي جوه الـ token (بنقراها بعد كده بـ User.GetUserId() من ClaimsExtensions)
            var clamis = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email,user.Email?? string.Empty)
            };

            // كل Role = Claim لوحده — ده اللي بيخلّي [Authorize(Roles = ...)] يشتغل
            clamis.AddRange(roles.Select(role=> new Claim(ClaimTypes.Role,role)));

            // وصف الـ token: مين صاحبه، صلاحيته قد إيه، مين اللي أصدره ولمين، واتوقّع بإيه
            // Issuer و Audience لازم يطابقوا ValidIssuer و ValidAudience في Program.cs
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(clamis),
                Expires = DateTime.UtcNow.AddDays(2),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                SigningCredentials = new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature)
            };

            // بيرجّع الـ token كـ string (header.payload.signature)
            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }
    }
}
