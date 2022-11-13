using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialDynamoAPI.BaseAggregator.Models
{
    public record CommentLike : Like
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Comment Comment { get; init; }
    }
}
