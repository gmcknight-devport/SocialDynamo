using Account.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Account.Models.Users
{
    public class User : IdentityUser
    {
        [Key]
        [MaxLength(20)]
        public int UserId { get; init; }

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

        public string RefreshToken { get; set; }
        public DateTime RefreshExpires { get; set; }

        public List<Follower> Followers { get; set; }
    }
}
