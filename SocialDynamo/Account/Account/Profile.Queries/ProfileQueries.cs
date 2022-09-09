using Account.API.ViewModels;
using Account.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Account.API.Infrastructure.Repositories;
using Xunit.Abstractions;
using Account.API.Common.ViewModels;
using Common;

namespace Account.API.Profile.Queries
{
    public class ProfileQueries : IProfileQueries
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly IFuzzySearch _fuzzySearch;
        private readonly ILogger<ProfileQueries> _logger;
                
        public ProfileQueries(IUserRepository userRepository, IFollowerRepository followerRepository,
                             IFuzzySearch fuzzySearch, ILogger<ProfileQueries> logger)
        {
            _userRepository = userRepository;
            _followerRepository = followerRepository;
            _fuzzySearch = fuzzySearch;
            _logger = logger;
        }

        public async Task<IActionResult> GetProfileInformation(string userId)
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

        public async Task<IEnumerable<UserDataVM>> SearchUser(string userId)
        {
            List<User>? users = await _fuzzySearch.FuzzySearch(userId) as List<User>;
            List<UserDataVM> userSearchResult = new();

            foreach(var user in users)
            {
                userSearchResult.Add(new UserDataVM
                {
                    UserId = user.UserId,
                    Forename = user.Forename,
                    Surname = user.Surname
                });
            }

            return userSearchResult;
        }

        public async Task<IEnumerable<UserDataVM>> GetUserFollowers(string userId)
        {
            var followerIds = await _followerRepository.GetFollowersAsync(userId);
            return await CreateFollowVM(followerIds);
        }

        public async Task<IEnumerable<UserDataVM>> GetUserFollowing(string userId)
        {
            var followingIds = await _followerRepository.GetUserFollowingAsync(userId);
            return await CreateFollowVM(followingIds);
        }

        private async Task<IEnumerable<UserDataVM>> CreateFollowVM(IEnumerable<Follower> followIds)
        {
            List<UserDataVM> following = new();

            foreach (var f in followIds)
            {
                User user = await _userRepository.GetUserAsync(f.FollowerId);

                following.Add(new UserDataVM
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
