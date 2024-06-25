using Common.API.Common.ViewModels;
using Common.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Common.API.Profile.Queries
{
    public interface IProfileQueries
    {
        Task<ProfileInformationVM> GetProfileInformation(string userId);
        Task<IEnumerable<ProfileInformationVM>> GetProfileInformation(IEnumerable<string> userIds);
        Task<IEnumerable<UserDataVM>> GetUserFollowers(string userId);
        Task<IEnumerable<UserDataVM>> GetUserFollowing(string userId);
        Task<IEnumerable<UserDataVM>> SearchUser(string userId);
    }
}