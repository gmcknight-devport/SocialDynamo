namespace Account.API.Commands
{
    public class RefreshJwtTokenCommand
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public int UserId { get; set; }
    }
}
