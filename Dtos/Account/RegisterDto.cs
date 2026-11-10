using System.ComponentModel.DataAnnotations;

namespace FinShark.Dtos.Account
{
    // Identity: البيانات اللي الـ client بيبعتها في POST /api/account/register
    // الـ DataAnnotations هنا للشكل بس — قواعد قوة الباسورد و Unique Email بيطبّقها Identity (من Program.cs)
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

    }
}
