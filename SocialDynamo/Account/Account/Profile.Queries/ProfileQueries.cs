using Account.API.ViewModels;
using Account.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Account.Domain.Repositories;
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

        /// <summary>
        /// Gets profile information for the specified user - forename, surname, 
        /// profile description, and number of followers
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetProfileInformation(string userId)
        {
            User user = await _userRepository.GetUserAsync(userId);
            var followers = await _followerRepository.GetFollowersAsync(userId);
            int numberOfFollowers = followers.Count();

            _logger.LogInformation("Returning profile information for user, User: {@user}", user);

            return new ObjectResult(new ProfileInformationVM
            {
                Forename = user.Forename,
                Surname = user.Surname,
                ProfileDescription = user.ProfileDescription,
                NumberOfFollowers = numberOfFollowers
            });
        }

        /// <summary>
        /// Uses fuzzy search to find the closest matches for a userId.
        /// Allows searching without having an exact value
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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
                    Surname = user.Surname,
                    ProfilePicture = user.ProfilePicture
                });
            }

            _logger.LogInformation("Attempting to return users found through fuzzy search, " +
                "Number found: {@users}", users.Count);

            return userSearchResult;
        }

        /// <summary>
        /// Gets all followers for specified user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDataVM>> GetUserFollowers(string userId)
        {
            var followerIds = await _followerRepository.GetFollowersAsync(userId);
            _logger.LogInformation("Getting followers for user, " +
               "User: {@userId}", userId);
                        
            return await CreateFollowVM(followerIds);
        }

        /// <summary>
        /// Returns all profiles the specified user is following
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDataVM>> GetUserFollowing(string userId)
        {
            var followingIds = await _followerRepository.GetUserFollowingAsync(userId);
            _logger.LogInformation("Getting user following, " +
               "User: {@userId}", userId);

            return await CreateFollowVM(followingIds);
        }

        /// <summary>
        /// Internal method to format the follower data for the view 
        /// model in order to return.
        /// </summary>
        /// <param name="followIds"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        private async Task<IEnumerable<UserDataVM>> CreateFollowVM(IEnumerable<Follower> followIds)
        {
            List<UserDataVM> following = new();

            foreach (var f in followIds)
            {
                var user = await _userRepository.GetUserAsync(f.FollowerId);

                following.Add(new UserDataVM
                {
                    UserId = user.UserId,
                    Forename = user.Forename,
                    Surname = user.Surname,
                    ProfilePicture = user.ProfilePicture
                });
            }
            if (following == null)
                throw new KeyNotFoundException();
            return following;
        }
    }
}
