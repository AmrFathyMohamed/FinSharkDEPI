using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FinShark.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public Collection<Comment> Comments { get; set; }
        public Collection<Wallet> Wallets { get; set; }
    }
}
