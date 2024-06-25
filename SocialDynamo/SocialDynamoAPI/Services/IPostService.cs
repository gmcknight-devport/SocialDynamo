using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.ViewModels;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    public interface IPostService
    {
        Task<bool> CreatePostAsync(CreatePostVM createPostVM, string httpCookie);
        Task<IActionResult> GetFeedAsync(string userId, int page, string httpCookie);
        Task<object> GetUserPosts(string userId, int page, string httpCookie);
    }
}