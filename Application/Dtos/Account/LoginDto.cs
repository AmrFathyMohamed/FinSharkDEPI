using System.ComponentModel.DataAnnotations;

namespace FinShark.Application.Dtos.Account
{
    // Identity: البيانات اللي الـ client بيبعتها في POST /api/account/login
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
