using Posts.Domain.Models;

namespace Posts.API.Queries
{
    public interface IPostsQueries
    {
        Task<IEnumerable<Post>> GetUserPostsAsync(string userId, int page);
        Task<IEnumerable<Post>> GetUsersPostsAsync(List<string> userIds, int page);
        Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId, int page);
        Task<IEnumerable<PostLike>> GetPostLikesAsync(Guid postId);
        Task<IEnumerable<CommentLike>> GetCommentLikesAsync(Guid commentId);
        Task<IEnumerable<Post>> FuzzySearchHashtag(string hashtag);
    }
}
