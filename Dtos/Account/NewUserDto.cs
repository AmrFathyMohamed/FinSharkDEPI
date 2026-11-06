namespace FinShark.Dtos.Account
{
    public class NewUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty;

    }
}
