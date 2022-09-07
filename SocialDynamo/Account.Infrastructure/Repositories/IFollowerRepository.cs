using Account.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Infrastructure.Repositories
{
    public interface IFollowerRepository
    {
        Task AddFollower(int userId, Follower follower);
        Task<IEnumerable<Follower>> GetFollowersAsync(int userId);
        Task<IEnumerable<Follower>> GetUserFollowingAsync(int userId);
        Task<IActionResult> GetFollowerCountAsync(int userId);
        Task RemoveFollower(Follower follower);
    }
}