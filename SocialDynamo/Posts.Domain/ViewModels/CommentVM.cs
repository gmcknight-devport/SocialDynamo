using Posts.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Posts.Domain.ViewModels
{
    public record CommentVM
    {
        [Required]
        public Guid CommentId { get; init; }

        [Required]
        [MaxLength(20)]
        public string AuthorId { get; init; }

        [Required]
        public DateTime PostedAt { get; init; }

        [Required]
        [MaxLength(2200)]
        public string CommentText { get; set; }

        [Required]
        public int LikeCount { get; set; }

        [Required]
        public ICollection<CommentLike>? Likes { get; set; }
    }
}
