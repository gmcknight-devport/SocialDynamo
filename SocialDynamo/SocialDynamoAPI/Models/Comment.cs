using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.Models
{
    public class Comment
    {
        [Required]
        [Key]
        public Guid CommentId { get; init; }

        [Required]
        [MaxLength(20)]
        public string AuthorId { get; init; }

        [Required]
        public DateTime PostedAt { get; init; }

        [Required]
        [MaxLength(2200)]
        public string CommentText { get; set; }

        public ICollection<CommentLike>? Likes { get; set; }

        public Post Post { get; set; }
    }
}
