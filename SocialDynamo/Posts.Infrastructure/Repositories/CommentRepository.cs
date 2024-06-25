using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Posts.Domain.Models;
using Posts.Domain.Repositories;
using Posts.Domain.ViewModels;
using Posts.Infrastructure.Persistence;

namespace Posts.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostsDbContext _postsDbContext;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(PostsDbContext postsDbContext, ILogger<CommentRepository> logger)
        {
            _postsDbContext = postsDbContext;
            _logger = logger;
        }

        public async Task<Guid> AddCommentAsync(Guid postId, Comment comment)
        {
            if (comment.Post == null)
                throw new ArgumentNullException("Comment doesn't have a valid Post");

            await _postsDbContext.Comments.AddAsync(comment);
            await _postsDbContext.SaveChangesAsync();

            return comment.CommentId;
        }

        public async Task DeleteCommentAsync(Guid commentId)
        {
            var comment = await _postsDbContext.Comments.Include(x => x.Likes).FirstAsync(x => x.CommentId == commentId);
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            _postsDbContext.Comments.Remove(comment);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<LikeVM>> GetCommentLikesAsync(Guid commentId)
        {
            var comment = await _postsDbContext.Comments.Include(x => x.Likes).FirstAsync(x => x.CommentId == commentId);
            if (comment == null)
                throw new ArgumentNullException("Couldn't find comment");

            List<LikeVM> likes = comment.Likes
                .Select(x => new LikeVM { Id = x.Id, LikeUserId = x.LikeUserId})
                .ToList();
            
            return likes;
        }

        public async Task<IEnumerable<CommentVM>> GetPostCommentsAsync(Guid postId, int page)
        {
            var post = await _postsDbContext.Posts.FindAsync(postId);
            var resultsPerPage = 10;

            if (post == null)
                throw new ArgumentNullException("Couldn't find post");

            var comments = await _postsDbContext.Comments.Where(c => c.Post == post)
                                                         .Select(x => new CommentVM{CommentId = x.CommentId, AuthorId = x.AuthorId, 
                                                                 PostedAt = x.PostedAt, CommentText = x.CommentText, 
                                                                 LikeCount = x.Likes.Count,
                                                                 Likes = x.Likes})
                                                         .OrderByDescending(c => c.PostedAt)
                                                         .Skip((page - 1) * resultsPerPage)
                                                         .Take(resultsPerPage)
                                                         .ToListAsync();

            if (comments == null)
                throw new ArgumentNullException("Couldn't find comments");

            return comments;
        }

        public async Task LikeCommentAsync(Guid commentId, string userId)
        {
            var comment = await _postsDbContext.Comments
                    .Include(c => c.Likes)
                    .FirstOrDefaultAsync(c => c.CommentId == commentId);

            if (comment == null)
                throw new ArgumentNullException("Couldn't find Comment");

            // Check if the user already liked the post
            var existingLike = comment.Likes.FirstOrDefault(like => comment.CommentId == commentId && like.LikeUserId == userId);

            if (existingLike != null)
                _postsDbContext.CommentLikes.Remove(existingLike);
            else
            {
                CommentLike like = new()
                {
                    Comment = comment,
                    LikeUserId = userId
                };
                await _postsDbContext.CommentLikes.AddAsync(like);
            }
                        
            await _postsDbContext.SaveChangesAsync();
        }
    }
}
