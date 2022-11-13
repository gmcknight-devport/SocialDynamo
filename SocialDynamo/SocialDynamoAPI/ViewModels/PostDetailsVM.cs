using SocialDynamoAPI.BaseAggregator.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public record PostDetailsVM
    {
        [Required]
        public string AuthorId { get; set; }

        public string Hashtag { get; set; }

        [Required]
        public string Caption { get; set; }

        [Required]
        public List<MediaItemId> MediaItemIds { get; set; }
    }
}
