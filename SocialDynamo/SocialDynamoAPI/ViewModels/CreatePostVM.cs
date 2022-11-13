using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public record CreatePostVM
    {
        [Required]
        public string AuthorId { get; set; }
        public string Hashtag { get; set; }

        [Required]
        public string Caption { get; set; }
        [Required]
        public List<IFormFile> Files { get; set; }
    }
}
