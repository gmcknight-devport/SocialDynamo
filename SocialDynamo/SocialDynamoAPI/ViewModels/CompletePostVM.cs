using SocialDynamoAPI.BaseAggregator.ValueObjects;
using SocialDynamoAPI.BaseAggregator.Models;
using System.ComponentModel.DataAnnotations;
using Azure;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public class CompletePostVM
    {
        [Required]
        public Guid PostId { get; init; }

        [Required]

        public string UserId { get; set; }

        [Required]

        public string Hashtag { get; set; }

        [Required]

        public string Caption { get; set; }

        [Required]

        public DateTime PostedAt { get; init; }

        [Required]

        public List<MediaItemId> MediaItemIds { get; set; }

        [Required]

        public List<byte[]> Files { get; set; }

        [Required]

        public ICollection<PostLike> Likes { get; set; }

        [Required]
        public ICollection<Comment> Comments { get; set; }

        public CompletePostVM(Post postDetailsVM, List<byte[]> mediaData)
        {
            PostId = postDetailsVM.PostId;
            UserId = postDetailsVM.AuthorId;
            if (postDetailsVM.Hashtag == null)
                postDetailsVM.Hashtag = "";
            Hashtag = postDetailsVM.Hashtag;
            Caption = postDetailsVM.Caption;
            PostedAt = postDetailsVM.PostedAt;
            MediaItemIds = (List<MediaItemId>)postDetailsVM.MediaItemIds;
            Files = mediaData;
            Likes = postDetailsVM.Likes;
            Comments = postDetailsVM.Comments;
        }
    }
}
