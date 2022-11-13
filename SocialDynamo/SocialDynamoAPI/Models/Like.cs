
namespace SocialDynamoAPI.BaseAggregator.Models
{
    public abstract record Like
    {
        public string LikeUserId { get; init; }
    }
}
