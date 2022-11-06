using Posts.Domain.Models;
using Posts.Domain.ViewModels;

namespace Posts.Domain.Repositories
{
    public interface ICommentRepository
    {
        public Task AddCommentAsync(Guid postId, Comment comment);
        public Task DeleteCommentAsync(Guid commentId);
        public Task LikeCommentAsync(Guid commentId, string userId);
        public Task<IEnumerable<CommentVM>> GetPostCommentsAsync(Guid postId, int page);
        public Task<IEnumerable<LikeVM>> GetCommentLikesAsync(Guid commentId);
    }   
}
