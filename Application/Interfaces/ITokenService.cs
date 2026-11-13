using FinShark.Domain.Entities.Models;

namespace FinShark.Application.Interfaces
{
    // Identity: عقد خدمة الـ JWT — الـ AccountController بيعتمد على الـ Interface مش على TokenService مباشرة
    // (نفس فكرة الـ Repositories في Session 5: سهل نغيّر التنفيذ أو نعمله Mock في الـ Tests)
    public interface ITokenService
    {
        // بيعمل JWT فيه بيانات الـ user والـ Roles بتاعته
        string CreateToken(AppUser user, IList<string> roles);
    }
}
