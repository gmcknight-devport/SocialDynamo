using Account.Infrastructure.Persistence;
using Account.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Account.API.Infrastructure.Repositories
{
    public class FollowerRepository : IFollowerRepository
    {
        private readonly AccountDbContext _accountDbContext;

        public FollowerRepository(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        public async Task<IEnumerable<Follower>> GetFollowersAsync(int userId)
        {
            var followers = await _accountDbContext.Followers
                .Include(f => f.FollowerId)
                .Where(u => u.UserId == userId).ToListAsync();

            return followers;
        }

        public async Task<IEnumerable<Follower>> GetUserFollowingAsync(int userId)
        {
            var following = await _accountDbContext.Followers
                .Include(f => f.FollowerId)
                .Where(f => f.FollowerId == userId).ToListAsync();

            return following;
        }

        public async Task<IActionResult> GetFollowerCountAsync(int userId)
        {
            int followerCount = await _accountDbContext.Followers
                .Include(f => f.FollowerId)
                .Where(u => u.UserId == userId)
                .CountAsync();

            return new ObjectResult(followerCount);
        }

        public async Task AddFollower(Follower follower)
        {
            if (follower == null)
            {
                throw new ArgumentNullException(nameof(follower));
            }

            _accountDbContext.Followers.Add(follower);
            await _accountDbContext.SaveChangesAsync();
        }

        public async Task RemoveFollower(Follower follower)
        {
            if (follower == null)
            {
                throw new ArgumentNullException(nameof(follower));
            }

            _accountDbContext.Followers.Remove(follower);
            await _accountDbContext.SaveChangesAsync();
        }
    }
}
