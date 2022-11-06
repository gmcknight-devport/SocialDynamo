using Posts.Domain.Models;
using Posts.Domain.ViewModels;

namespace Posts.Domain.Repositories
{
    public interface IPostRepository
    {
        public Task CreatePostAsync(Post post);
        public Task DeletePostAsync(Guid postId);
        public Task LikePostAsync(Guid postId, string userId);
        public Task<Post> GetPostAsync(Guid postId);
        public Task<IEnumerable<Post>> GetUserPostsAsync(string userId, int page);
        public Task<IEnumerable<Post>> GetUsersPostsAsync(List<string> userIds, int page);
        public Task<IEnumerable<LikeVM>> GetPostLikesAsync(Guid postId);
        public Task UpdatePostAsync(Post post);
    }
}
