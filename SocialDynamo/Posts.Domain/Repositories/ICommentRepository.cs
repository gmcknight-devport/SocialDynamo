using Posts.Domain.Models;

namespace Posts.Domain.Repositories
{
    public interface ICommentRepository
    {
        public Task AddCommentAsync(Guid postId, Comment comment);
        public Task DeleteCommentAsync(Guid commentId);
        public Task LikeCommentAsync(Guid commentId, string userId);
        public Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId, int page);
        public Task<IEnumerable<CommentLike>> GetCommentLikesAsync(Guid commentId);
    }   
}
