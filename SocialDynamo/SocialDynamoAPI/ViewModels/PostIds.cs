using SocialDynamoAPI.BaseAggregator.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public class PostIds
    {
        [Required]
        public Guid PostId { get; set; }

        [Required]
        public MediaItemId MediaItemId { get; set; }
    }
}
