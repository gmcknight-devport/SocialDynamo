using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.ViewModels;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    public interface IPostService
    {
        Task<bool> CreatePostAsync(CreatePostVM createPostVM);
        Task<IActionResult> GetFeedAsync(string userId, int page);
        Task<IActionResult> GetUserPosts(string userId, int page);
    }
}