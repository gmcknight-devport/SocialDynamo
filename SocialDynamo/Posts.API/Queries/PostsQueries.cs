using Common;
using Posts.Domain.Models;
using Posts.Domain.Repositories;
using Posts.Domain.ViewModels;

namespace Posts.API.Queries
{
    public class PostsQueries : IPostsQueries
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IFuzzySearch _fuzzySearch;
        private readonly ILogger<PostsQueries> _logger;

        public PostsQueries(IPostRepository postRepository, ICommentRepository commentRepository,
                            IFuzzySearch fuzzySearch, ILogger<PostsQueries> logger)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _fuzzySearch = fuzzySearch;
            _logger = logger;
        }

        /// <summary>
        /// Uses fuzzy search to find the closest matches for a hashtag.
        /// Allows searching without having an exact value
        /// </summary>
        /// <param name="hashtag"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Post>> FuzzySearchHashtag(string hashtag)
        {
            List<Post>? hashtagPosts = await _fuzzySearch.FuzzySearch(hashtag) as List<Post>;
            _logger.LogInformation("Attempting to return posts found with hashtag through fuzzy search, " +
                "Number found: {@hashtagPosts}", hashtagPosts);

            return hashtagPosts;
        }

        /// <summary>
        /// Get likes for a comment. 
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LikeVM>> GetCommentLikesAsync(Guid commentId)
        {
            var commentLikes = await _commentRepository.GetCommentLikesAsync(commentId);
            _logger.LogInformation("Returning comment likes. Comment: {@commentId}", commentId);

            return commentLikes;
        }

        /// <summary>
        /// Get comments for a post in pages. 
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CommentVM>> GetPostCommentsAsync(Guid postId, int page)
        {
            var postComments = await _commentRepository.GetPostCommentsAsync(postId, page);
            _logger.LogInformation("Returning post comments. Post: {@postId}, " +
                "Page number: {@page}", postId, page);

            return postComments;
        }

        /// <summary>
        /// Get likes for a post. 
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LikeVM>> GetPostLikesAsync(Guid postId)
        {
            var postLikes = await _postRepository.GetPostLikesAsync(postId);
            _logger.LogInformation("Returning post likes. Post: {@postId}", postId);

            return postLikes;
        }

        /// <summary>
        /// Returning posts for the specified user in pages. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Post>> GetUserPostsAsync(string userId, int page)
        {
            var userPosts = await _postRepository.GetUserPostsAsync(userId, page);
            _logger.LogInformation("Returning user posts. User: {@userId}, " +
                "Post page: {@page}", userId, page);

            return userPosts;
        }

        /// <summary>
        /// Returns posts of multiple users in pages. 
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Post>> GetUsersPostsAsync(List<string> userIds, int page)
        {
            var usersPosts = await _postRepository.GetUsersPostsAsync(userIds, page);
            _logger.LogInformation("Returning users posts. Users: {@userIds}, " +
                "Post page: {@page}", userIds, page);

            return usersPosts;
        }
    }
}
