using System.ComponentModel.DataAnnotations;

namespace Account.API.Commands
{
    public class RegisterUserCommand
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        [RegularExpression("(?i)^[a-zA-Z0-9]*$")]
        public string UserId { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
    }
}
