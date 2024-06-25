using SocialDynamoAPI.BaseAggregator.ValueObjects;
using SocialDynamoAPI.BaseAggregator.Models;
using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public class CompletePostVM
    {
        [Required]
        public Guid PostId { get; init; }

        [Required]

        public string UserId { get; set; }

        [Required]

        public string UsersName { get; set; }

        [MaxLength(15 * 1024 * 1024)]
        public byte[] ProfilePicture { get; set; } = new byte[1];

        [Required]

        public string Hashtag { get; set; }

        [Required]

        public string Caption { get; set; }

        [Required]

        public DateTime PostedAt { get; init; }

        [Required]

        public List<MediaItemId> MediaItemIds { get; set; }

        [Required]

        public List<Uri> Files { get; set; }

        [Required]

        public ICollection<PostLike> Likes { get; set; }

        [Required]
        public ICollection<Comment> Comments { get; set; }

        public CompletePostVM(Post post, UserDataVM userDataVM, List<Uri> mediaData)
        {
            PostId = post.PostId;
            UserId = post.AuthorId;
            UsersName = userDataVM.Forename + " " + userDataVM.Surname;
            ProfilePicture = userDataVM.ProfilePicture;
            if (post.Hashtag == null)
                post.Hashtag = "";
            Hashtag = post.Hashtag;
            Caption = post.Caption;
            PostedAt = post.PostedAt;
            MediaItemIds = (List<MediaItemId>)post.MediaItemIds;
            Files = mediaData;
            Likes = post.Likes;
            Comments = post.Comments;
        }
    }
}
