using Account.API.Common.ViewModels;
using Account.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Profile.Queries
{
    public interface IProfileQueries
    {
        Task<IActionResult> GetProfileInformation(int userId);
        Task<IEnumerable<FollowDataVM>> GetUserFollowers(int userId);
        Task<IEnumerable<FollowDataVM>> GetUserFollowing(int userId);
    }
}