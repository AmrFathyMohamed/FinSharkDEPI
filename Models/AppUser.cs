using Microsoft.AspNetCore.Identity;

namespace FinShark.Models
{
    public class AppUser : IdentityUser
    {
        public List<Portolio> Portolios { get; set; } = new List<Portolio>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
