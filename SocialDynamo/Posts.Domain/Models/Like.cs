
namespace Posts.Domain.Models
{
    public abstract record Like
    {
        public string LikeUserId { get; init; }
    }
}
