using Account.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Models.Users
{
    public class Follower
    {
        [ForeignKey("UserForeignKey")]
        [MaxLength(20)]
        [Required]
        public int UserId;

        [MaxLength(20)]
        [Required]
        public int FollowerId { get; set; }

        public User User { get; set; }
    }
}
