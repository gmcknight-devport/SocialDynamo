using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Models.Users
{
    public class Follower
    {
        [MaxLength(100)]
        [Key]
        [Required]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string FollowerId { get; set; }

        [ForeignKey("FollowerId")]
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
