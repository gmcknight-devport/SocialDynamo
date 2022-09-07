using Account.API.ViewModels;
using Account.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Account.API.Infrastructure.Repositories;
using Xunit.Abstractions;
using Account.API.Common.ViewModels;

namespace Account.API.Profile.Queries
{
    public class ProfileQueries : IProfileQueries
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly ILogger<ProfileQueries> _logger;
                
        public ProfileQueries(IUserRepository userRepository, IFollowerRepository followerRepository,
                             ILogger<ProfileQueries> logger)
        {
            _userRepository = userRepository;
            _followerRepository = followerRepository;
            _logger = logger;
        }

        public async Task<IActionResult> GetProfileInformation(int userId)
        {
            User user = await _userRepository.GetUserAsync(userId);
            var followers = await _followerRepository.GetFollowersAsync(userId);
            int numberOfFollowers = followers.Count();

            return new ObjectResult(new ProfileInformationVM
            {
                Forename = user.Forename,
                Surname = user.Surname,
                ProfileDescription = user.ProfileDescription,
                NumberOfFollowers = numberOfFollowers
            });
        }

        public async Task<IEnumerable<FollowDataVM>> GetUserFollowers(int userId)
        {
            var followerIds = await _followerRepository.GetFollowersAsync(userId);
            return await CreateFollowVM(followerIds);
        }

        public async Task<IEnumerable<FollowDataVM>> GetUserFollowing(int userId)
        {
            var followingIds = await _followerRepository.GetUserFollowingAsync(userId);
            return await CreateFollowVM(followingIds);
        }

        private async Task<IEnumerable<FollowDataVM>> CreateFollowVM(IEnumerable<Follower> followIds)
        {
            List<FollowDataVM> following = new();

            foreach (var f in followIds)
            {
                User user = await _userRepository.GetUserAsync(f.FollowerId);

                following.Add(new FollowDataVM
                {
                    UserId = user.UserId,
                    Forename = user.Forename,
                    Surname = user.Surname
                });
            }
            if (following == null)
                throw new KeyNotFoundException();
            return following;
        }
    }
}
