using Account.API.Common.ViewModels;
using Account.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Profile.Queries
{
    public interface IProfileQueries
    {
        Task<IActionResult> GetProfileInformation(string userId);
        Task<IEnumerable<ObjectResult>> GetUserFollowers(string userId);
        Task<IEnumerable<ObjectResult>> GetUserFollowing(string userId);
        Task<IEnumerable<UserDataVM>> SearchUser(string userId);
    }
}