using Common;
using Posts.Domain.Models;
using Posts.Infrastructure.Repositories;

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

        public async Task<IEnumerable<Post>> FuzzySearchHashtag(string hashtag)
        {
            List<Post>? hashtagPosts = await _fuzzySearch.FuzzySearch(hashtag) as List<Post>;
            return hashtagPosts;
        }

        public async Task<IEnumerable<CommentLike>> GetCommentLikesAsync(Guid commentId)
        {
            var commentLikes = await _commentRepository.GetCommentLikesAsync(commentId);
            return commentLikes;
        }

        public async Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId, int page)
        {
            var postComments = await _commentRepository.GetPostCommentsAsync(postId, page);
            return postComments;
        }

        public async Task<IEnumerable<PostLike>> GetPostLikesAsync(Guid postId)
        {
            var postLikes = await _postRepository.GetPostLikesAsync(postId);
            return postLikes;
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(string userId, int page)
        {
            var userPosts = await _postRepository.GetUserPostsAsync(userId, page);
            return userPosts;
        }

        public async Task<IEnumerable<Post>> GetUsersPostsAsync(List<string> userIds, int page)
        {
            var usersPosts = await _postRepository.GetUsersPostsAsync(userIds, page);
            return usersPosts;
        }
    }
}
