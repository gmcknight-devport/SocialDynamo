using Microsoft.EntityFrameworkCore;
using Posts.Domain.Models;
using Posts.Domain.Repositories;
using Posts.Domain.ViewModels;
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
            if (comment.Post == null)
            {
                throw new ArgumentNullException("Comment doesn't have a valid Post");
            }

            await _postsDbContext.Comments.AddAsync(comment);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Guid commentId)
        {
            var comment = await _postsDbContext.Comments.Include(x => x.Likes).FirstAsync(x => x.CommentId == commentId);
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            _postsDbContext.Comments.Remove(comment);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<LikeVM>> GetCommentLikesAsync(Guid commentId)
        {
            var comment = await _postsDbContext.Comments.Include(x => x.Likes).FirstAsync(x => x.CommentId == commentId);
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            List<LikeVM> likes = comment.Likes
                .Select(x => new LikeVM { Id = x.Id, LikeUserId = x.LikeUserId})
                .ToList();
            
            return likes;
        }

        public async Task<IEnumerable<CommentVM>> GetPostCommentsAsync(Guid postId, int page)
        {
            var post = await _postsDbContext.Posts.FindAsync(postId);
            var resultsPerPage = 10;

            var comments = await _postsDbContext.Comments.Where(c => c.Post == post)
                                                         .Select(x => new CommentVM{CommentId = x.CommentId, AuthorId = x.AuthorId, 
                                                                 PostedAt = x.PostedAt, CommentText = x.CommentText, 
                                                                 LikeCount = x.Likes.Count })
                                                         .OrderByDescending(c => c.PostedAt)
                                                         .Skip((page - 1) * resultsPerPage)
                                                         .Take(resultsPerPage)
                                                         .ToListAsync();
                        
            return comments;
        }

        public async Task LikeCommentAsync(Guid commentId, string userId)
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

            await _postsDbContext.CommentLikes.AddAsync(like);
            await _postsDbContext.SaveChangesAsync();
        }
    }
}
