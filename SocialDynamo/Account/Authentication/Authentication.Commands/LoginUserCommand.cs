using System.ComponentModel.DataAnnotations;

namespace Account.API.Commands
{
    public class LoginUserCommand
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
