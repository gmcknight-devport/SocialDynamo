using SocialDynamoAPI.BaseAggregator.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.Models
{
    public class Post
    {
        [Required]
        [Key]
        public Guid PostId { get; init; }

        [Required]
        [MaxLength(20)]
        public string AuthorId { get; init; }

        [RegularExpression("/ (^|)#(?![0-9]+\b)([a-zA-Z0-9]{1,30})(\b|\r)/g")]
        [MaxLength(120)]
        public string Hashtag { get; set; }

        [Required]
        [MaxLength(2200)]
        public string Caption { get; set; }

        [Required]
        public DateTime PostedAt { get; init; }

        [Required]
        public ICollection<MediaItemId> MediaItemIds { get; set; }

        public ICollection<PostLike> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
