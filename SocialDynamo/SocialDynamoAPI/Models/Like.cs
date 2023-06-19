
using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.Models
{
    public abstract record Like
    {
        [Required]
        public string LikeUserId { get; init; }
    }
}
