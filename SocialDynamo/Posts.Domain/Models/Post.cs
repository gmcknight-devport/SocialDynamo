using Posts.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Models
{
    public class Post
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public ICollection<MediaItemId> MediaItemIds{ get; set; }

        public ICollection<PostLike>? Likes { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
