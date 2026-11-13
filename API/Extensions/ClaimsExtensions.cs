using System.Security.Claims;

namespace FinShark.API.Extensions
{
    // Identity: Extension Methods على User (الـ ClaimsPrincipal) جوه أي Controller.
    // بدل ما نكتب User.FindFirstValue(ClaimTypes.NameIdentifier) كل مرة → User.GetUserId()
    // القيم دي جاية من الـ Claims اللي TokenService حطها في الـ JWT
    public static class ClaimsExtensions
    {
        // Id الـ user اللي عامل login (من الـ Claim: NameIdentifier)
        public static string? GetUserId (this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        // الـ UserName بتاع الـ user اللي عامل login (من الـ Claim: Name)
        public static string? GetUserName (this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }
    }
}
