using Posts.Domain.Models;

namespace Posts.API.Queries
{
    public interface IPostsQueries
    {
        Task<IEnumerable<Post>> GetUserPostsAsync(int userId);
        Task<IEnumerable<Post>> GetUsersPostsAsync(List<int> userIds);
        Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId);
        Task<IEnumerable<PostLike>> GetPostLikesAsync(Guid postId);
        Task<IEnumerable<CommentLike>> GetCommentLikesAsync(Guid commentId);
        Task<IEnumerable<Post>> FuzzySearchHashtag(string hashtag);
    }
}
