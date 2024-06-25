using System.ComponentModel.DataAnnotations;

namespace Common.API.Commands
{
    public class LogoutUserCommand
    {
        [Required]
        public string UserId { get; set; }
    }
}
