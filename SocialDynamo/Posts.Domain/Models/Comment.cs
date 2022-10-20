using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posts.Domain.Models
{
    public class Comment
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
