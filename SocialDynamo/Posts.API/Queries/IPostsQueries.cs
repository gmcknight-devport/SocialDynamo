using Posts.Domain.Models;
using Posts.Domain.ViewModels;

namespace Posts.API.Queries
{
    public interface IPostsQueries
    {
        Task<Post> GetPost(Guid postId);
        Task<IEnumerable<Post>> GetUserPostsAsync(string userId, int page);
        Task<IEnumerable<Post>> GetUsersPostsAsync(List<string> userIds, int page);
        Task<IEnumerable<CommentVM>> GetPostCommentsAsync(Guid postId, int page);
        Task<IEnumerable<LikeVM>> GetPostLikesAsync(Guid postId);
        Task<IEnumerable<LikeVM>> GetCommentLikesAsync(Guid commentId);
        Task<IEnumerable<IEnumerable<LikeVM>>> GetCommentsLikesAsync(List<Guid> commentIds);
        Task<IEnumerable<Post>> FuzzySearchHashtag(string hashtag);
    }
}
