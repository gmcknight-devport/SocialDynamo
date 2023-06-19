using Common;
using Microsoft.EntityFrameworkCore;
using Posts.Domain.Models;
using Posts.Domain.Repositories;
using Posts.Domain.ViewModels;
using Posts.Infrastructure.Persistence;

namespace Posts.Infrastructure.Repositories
{   
    public class PostRepository : IPostRepository, IFuzzySearch
    {
        private readonly PostsDbContext _postsDbContext;

        public PostRepository(PostsDbContext postsDbContext)
        {
            _postsDbContext = postsDbContext;
        }
        public async Task CreatePostAsync(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            _postsDbContext.Posts.Add(post);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task DeletePostAsync(Guid postId)
        {
            var post = await GetPostAsync(postId);

            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            _postsDbContext.Posts.Remove(post);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task<Post> GetPostAsync(Guid postId)
        {
            var post = await _postsDbContext.Posts
                    .Include(p => p.MediaItemIds)
                    .Include(p => p.Likes)
                    .Include(p => p.Comments)
                    .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            return post;
        }

        public async Task<IEnumerable<LikeVM>> GetPostLikesAsync(Guid postId)
        {
            var post = await GetPostAsync(postId);

            if (post == null)
                throw new ArgumentNullException(nameof(post));

            var postLikes = post.Likes.Select(x => new LikeVM
            {
                Id = x.Id,
                LikeUserId = x.LikeUserId
            }).ToList();

            return postLikes;
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(string userId, int page)
        {
            int resultsPerPage = 12;
            List<Post> posts = await _postsDbContext.Posts                                    
                                    .Include(p => p.Comments)
                                    .Include(p => p.MediaItemIds)
                                    .Include(p => p.Likes)
                                    .Where(p => p.AuthorId == userId)  
                                    .OrderByDescending(p => p.PostedAt)                                    
                                    .Skip((page - 1) * resultsPerPage)
                                    .Take(resultsPerPage)
                                    .ToListAsync();

            if(posts == null)
            {
                throw new ArgumentNullException(nameof(posts));
            }

            return posts;
        }

        public async Task<IEnumerable<Post>> GetUsersPostsAsync(List<string> userIds, int page)
        {
            int resultsPerPage = 10;
            List<Post> posts = await _postsDbContext.Posts
                                                    .Include(p => p.Comments)
                                                    .Include(p => p.MediaItemIds)
                                                    .Include(p => p.Likes)
                                                    .Where(p => userIds.Contains(p.AuthorId))
                                                    .OrderByDescending(p => p.PostedAt)
                                                    .Skip((page - 1) * resultsPerPage)
                                                    .Take(resultsPerPage)
                                                    .ToListAsync();

            if (posts == null)
            {
                throw new ArgumentNullException(nameof(posts));
            }

            return posts;
        }

        public async Task LikePostAsync(Guid postId, string userId)
        {
            var post = await GetPostAsync(postId);
           
            if(post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            PostLike like = new()
            {
                Post = post, 
                LikeUserId = userId
            };

            await _postsDbContext.PostLikes.AddAsync(like);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(Post post)
        {
            var currentPost = await GetPostAsync(post.PostId);
            if(currentPost == null)
            {
                throw new ArgumentNullException(nameof(currentPost));
            }

            _postsDbContext.Update(post);
            await _postsDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> FuzzySearch(string fuzzyHashtag)
        {
            var results = await _postsDbContext.Posts
                                               .Include(p => p.Comments)
                                               .Include(p => p.MediaItemIds)
                                               .Include(p => p.Likes)
                                               .Where(d => EF.Functions.Like(d.Hashtag, "%" + fuzzyHashtag + "%"))
                                               .Take(5).ToListAsync();
            return results;
        }
    }
}