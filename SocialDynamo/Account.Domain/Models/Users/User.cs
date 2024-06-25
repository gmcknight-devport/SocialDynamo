using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Users
{
    public class User
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
        [MaxLength(64)]
        public string Password { get; set; }

        [MaxLength(15 * 1024 * 1024)]
        public byte[] ProfilePicture { get; set; } = new byte[1];

        [Required]
        [MaxLength(50)]
        public string Forename { get; set; }

        [Required]
        [MaxLength(50)]
        public string Surname { get; set; }

        public string ProfileDescription { get; set; }

        [MaxLength(120)]
        public string RefreshToken { get; set; }
        public DateTime RefreshExpires { get; set; }

        [ForeignKey("UserId")]
        public ICollection<Follower> Followers { get; set; }
    }
}
