using System.ComponentModel.DataAnnotations;

namespace Account.API.Commands
{
    public class RefreshJwtTokenCommand
    {
        [Required]
        public string? Token { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
