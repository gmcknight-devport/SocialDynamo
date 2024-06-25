using Common.Domain.Repositories;
using Common.Infrastructure.Persistence;
using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Common.API.Infrastructure.Repositories
{
    public class FollowerRepository : IFollowerRepository
    {
        private readonly AccountDbContext _accountDbContext;

        public FollowerRepository(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        public async Task<IEnumerable<Follower>> GetFollowersAsync(string userId)
        {
            var followers = await _accountDbContext.Users
                .Where(u => u.UserId == userId)
                .SelectMany(u => u.Followers)
                .ToListAsync();

            return followers;
        }

        public async Task<IEnumerable<Follower>> GetUserFollowingAsync(string userId)
        {
            var userIds = await _accountDbContext.Followers
                                                   .Where(follower => follower.FollowerId == userId)
                                                   .SelectMany(follower => follower.Users)
                                                   .ToListAsync();

            var followers = userIds.Select(id => new Follower { FollowerId = id.UserId }).ToList();
            return followers;
        }

        public async Task<IActionResult> GetFollowerCountAsync(string userId)
        {
            int followerCount = await _accountDbContext.Users                
                .Where(u => u.UserId == userId)
                .Select(u => u.Followers)
                .CountAsync();

            return new ObjectResult(followerCount);
        }

        public async Task AddFollower(string userId, Follower follower)
        {
            //Check if user exists
            var user = await _accountDbContext.Users.Include(x => x.Followers).SingleAsync(x => x.UserId == userId);                
            if (user == null) throw new ArgumentNullException("Error getting User.");

            //Check if follower exists and create if not
            bool exists = await _accountDbContext.Followers.AnyAsync(f => f.FollowerId == follower.FollowerId);
            if (!exists)
            {
                await _accountDbContext.Followers.AddAsync(follower);
                await _accountDbContext.SaveChangesAsync();
            }

            //Check if following user
            var followingUser = user.Followers.FirstOrDefault(f => f.FollowerId == follower.FollowerId);

            if (followingUser != null)
                user.Followers.Remove(followingUser);
            else
            {
                var existingFollower = await _accountDbContext.Followers.SingleAsync(f => f.FollowerId == follower.FollowerId);
                user.Followers.Add(existingFollower);
            }

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
