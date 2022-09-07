using Posts.Domain.Models;

namespace Posts.Infrastructure.Repositories
{
    public interface ICommentRepository
    {
        public Task AddCommentAsync(Guid postId, Comment comment);
        public Task DeleteCommentAsync(Guid commentId);
        public Task LikeCommentAsync(Guid commentId, int userId);
        public Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId);
        public Task<IEnumerable<CommentLike>> GetCommentLikesAsync(Guid commentId);
    }   
}
