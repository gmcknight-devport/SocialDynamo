using Posts.Domain.Models;
using Posts.Infrastructure.Repositories;

namespace Posts.API.Queries
{
    public class PostsQueries : IPostsQueries
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<PostsQueries> _logger;

        public PostsQueries(IPostRepository postRepository, ICommentRepository commentRepository, ILogger<PostsQueries> logger)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public Task<IEnumerable<CommentLike>> GetCommentLikesAsync(Guid commentId)
        {
            var commentLikes = _commentRepository.GetCommentLikesAsync(commentId);
            return commentLikes;
        }

        public Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId)
        {
            var postComments = _commentRepository.GetPostCommentsAsync(postId);
            return postComments;
        }

        public Task<IEnumerable<PostLike>> GetPostLikesAsync(Guid postId)
        {
            var postLikes = _postRepository.GetPostLikesAsync(postId);
            return postLikes;
        }

        public Task<IEnumerable<Post>> GetUserPostsAsync(int userId)
        {
            var userPosts = _postRepository.GetUserPostsAsync(userId);
            return userPosts;
        }

        public Task<IEnumerable<Post>> GetUsersPostsAsync(List<int> userIds)
        {
            var usersPosts = _postRepository.GetUsersPostsAsync(userIds);
            return usersPosts;
        }
    }
}
