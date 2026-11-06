using System.ComponentModel.DataAnnotations;

namespace FinShark.Dtos.Account
{
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
