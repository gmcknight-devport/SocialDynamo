using SocialDynamoAPI.BaseAggregator.ValueObjects;
using SocialDynamoAPI.BaseAggregator.Models;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public class CompletePostVM
    {
        public Guid PostId { get; init; }

        public string UserId { get; set; }

        public string Hashtag { get; set; }

        public string Caption { get; set; }

        public DateTime PostedAt { get; init; }

        public List<MediaItemId> MediaItemIds { get; set; }

        public List<BinaryData> Files { get; set; }

        public ICollection<PostLike> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public CompletePostVM(Post postDetailsVM, List<BinaryData> mediaData)
        {
            PostId = postDetailsVM.PostId;
            UserId = postDetailsVM.AuthorId;
            Hashtag = postDetailsVM.Hashtag;
            Caption = postDetailsVM.Hashtag;
            PostedAt = postDetailsVM.PostedAt;
            MediaItemIds = (List<MediaItemId>)postDetailsVM.MediaItemIds;
            Files = mediaData;
            Likes = postDetailsVM.Likes;
            Comments = postDetailsVM.Comments;
        }
    }
}
