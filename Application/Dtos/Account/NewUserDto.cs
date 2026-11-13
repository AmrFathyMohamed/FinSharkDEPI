namespace FinShark.Application.Dtos.Account
{
    // Identity: الرد اللي بيرجع بعد Register أو Login.
    // بنرجّع ده بدل AppUser عشان AppUser فيه PasswordHash و SecurityStamp ومينفعش يطلعوا للـ client
    public class NewUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        // الـ JWT — الـ client بيبعته بعد كده في الـ header: Authorization: Bearer <token>
        public string Token { get; set; } = string.Empty;

    }
}
