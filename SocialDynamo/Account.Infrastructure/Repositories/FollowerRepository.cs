using Account.Domain.Repositories;
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
            var users = await _accountDbContext.Users
                                                    .Where(user => user.Followers.SelectMany(follower => follower.Users)
                                                    .SelectMany(user => user.Followers)
                                                    .Select(user => user.FollowerId)
                                                    .Contains(userId))                                                    
                                                    .ToListAsync();
            
            List<Follower> followers = users.Select(x => new Follower { FollowerId = x.UserId }).ToList();

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
            if (user == null) throw new ArgumentNullException(nameof(user));

            //Check if follower exists and create if not
            bool exists = await _accountDbContext.Followers.AnyAsync(f => f.FollowerId == follower.FollowerId);
            if (!exists)
            {
                await _accountDbContext.Followers.AddAsync(follower);
                await _accountDbContext.SaveChangesAsync();
            }

            //Check if follows user and add link if not
            bool followsUser = user.Followers.Any(f => f.FollowerId == follower.FollowerId);            

            if (!followsUser)
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
