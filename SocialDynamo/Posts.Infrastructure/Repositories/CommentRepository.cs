using Microsoft.EntityFrameworkCore;
using Posts.Domain.Models;
using Posts.Infrastructure.Persistence;

namespace Posts.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostsDbContext _postsDbContext;

        public CommentRepository(PostsDbContext postsDbContext)
        {
            _postsDbContext = postsDbContext;
        }

        public async Task AddCommentAsync(Guid postId, Comment comment)
        {                                   
            var post = await _postsDbContext.Posts.FindAsync(postId);
            
            if(post != null)
                throw new ArgumentNullException(nameof(post));
            else if (comment == null)
                throw new ArgumentNullException(nameof(comment));
            else if(post.Comments == null)
                post.Comments = new List<Comment>();

            post.Comments.Add(comment);

            await _postsDbContext.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Guid commentId)
        {
            var comment = await _postsDbContext.Comments.FindAsync(commentId);
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            _postsDbContext.Comments.Remove(comment);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentLike>> GetCommentLikesAsync(Guid commentId)
        {
            var comment = await _postsDbContext.Comments.FindAsync(commentId);

            List<CommentLike> likes = await _postsDbContext.CommentLikes
                .Where(p => p.Comment == comment).ToListAsync();
            
            return likes;
        }

        public async Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId)
        {
            var post = await _postsDbContext.Posts.FindAsync(postId);

            List<Comment> comments = await _postsDbContext.Comments
                                                          .Where(c => c.Post == post)
                                                          .OrderByDescending(c => c.PostedAt)
                                                          .ToListAsync();

            return comments;
        }

        public async Task LikeCommentAsync(Guid commentId, int userId)
        {
            var comment = await _postsDbContext.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            CommentLike like = new()
            {
                Comment = comment,
                LikeUserId = userId
            };

            await _postsDbContext.SaveChangesAsync();
        }
    }
}
