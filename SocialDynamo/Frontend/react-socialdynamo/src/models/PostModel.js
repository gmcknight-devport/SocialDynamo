class PostModel {
  constructor(post) {
    this.PostId = post.PostId;
    this.UserId = post.AuthorId;
    this.UsersName = post.UsersName;
    this.ProfilePicture = post.ProfilePicture;
    this.Hashtag = post.Hashtag;
    this.Caption = post.Caption;
    this.PostedAt = post.PostedAt;
    this.MediaItemIds = post.MediaItemIds;
    this.Files = post.mediaData;
    this.Likes = post.Likes;
    this.Comments = post.Comments;
  }
}