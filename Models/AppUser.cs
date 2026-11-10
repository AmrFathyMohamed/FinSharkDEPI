using Microsoft.AspNetCore.Identity;

namespace FinShark.Models
{
    // Identity: الـ User بتاعنا — بيورث من IdentityUser فبياخد جاهز:
    // Id, UserName, Email, PasswordHash, SecurityStamp, LockoutEnd... (جدول AspNetUsers)
    // وبنزوّد عليه الحاجات الخاصة بالمشروع بس
    public class AppUser : IdentityUser
    {
        // One-to-Many: الـ user ليه كذا سهم في الـ Portfolio بتاعه
        public List<Portolio> Portolios { get; set; } = new List<Portolio>();
        // One-to-Many: الـ user ليه تعليقات كتير (EF عامل عمود AppUserId في جدول Comments كـ Shadow FK)
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
