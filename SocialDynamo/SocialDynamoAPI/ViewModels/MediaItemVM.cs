using SocialDynamoAPI.BaseAggregator.Extensions;
using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public record MediaItemVM
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string MediaItemId { get; set; }
        [Required]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".mp4", ".mov" })]
        public IFormFile File { get; set; }
    }
}
