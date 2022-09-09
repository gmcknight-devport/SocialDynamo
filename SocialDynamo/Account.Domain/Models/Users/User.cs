using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Account.Models.Users
{
    public class User : IdentityUser
    {
        [Key]
        [MaxLength(100)]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string UserId { get; init; }

        [EmailAddress]
        [Required]
        [MaxLength(320)]
        public string EmailAddress { get; init; }

        [Required]
        [MaxLength(24)]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string Forename { get; set; }

        [Required]
        [MaxLength(50)]
        public string Surname { get; set; }

        public string ProfileDescription { get; set; }

        [MaxLength(36)]
        public string RefreshToken { get; set; }
        public DateTime RefreshExpires { get; set; }

        public List<Follower> Followers { get; set; }
    }
}
