using Posts.Domain.Models;

namespace Posts.Infrastructure.Repositories
{
    public interface IPostRepository
    {
        public Task CreatePostAsync(Post post);
        public Task DeletePostAsync(Guid postId);
        public Task LikePostAsync(Guid postId, int userId);
        public Task<Post> GetPostAsync(Guid postId);
        public Task<IEnumerable<Post>> GetUserPostsAsync(int userId, int page);
        public Task<IEnumerable<Post>> GetUsersPostsAsync(List<int> userIds, int page);
        public Task<IEnumerable<PostLike>> GetPostLikesAsync(Guid postId);
        public Task UpdatePostAsync(Post post);
    }
}
