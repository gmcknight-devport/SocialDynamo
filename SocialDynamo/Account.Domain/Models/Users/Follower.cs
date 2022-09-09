using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Models.Users
{
    public class Follower
    {
        [ForeignKey("UserForeignKey")]
        [MaxLength(100)]
        [Required]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string UserId;

        [MaxLength(100)]
        [Required]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string FollowerId { get; set; }

        public User User { get; set; }
    }
}
