using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public class ProfilePagePostVM
    {
        [Required]
        public string UserId { get; init; }
        [Required]
        public Guid PostId { get; init; }
              

        [Required]
        public Uri File { get; init; }
        
        public ProfilePagePostVM(string userId, Guid postId, Uri mediaData)
        {
            UserId = userId;
            PostId = postId;
            File = mediaData;
        }
    }
}
