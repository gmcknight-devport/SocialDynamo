using System.ComponentModel.DataAnnotations;

namespace Common.API.Commands
{
    public class LoginUserCommand
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
