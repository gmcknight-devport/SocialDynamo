using System.ComponentModel.DataAnnotations;

namespace Account.API.Commands
{
    public class LogoutUserCommand
    {
        [Required]
        public string UserId { get; set; }
    }
}
