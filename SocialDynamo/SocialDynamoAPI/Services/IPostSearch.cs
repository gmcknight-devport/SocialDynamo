using SocialDynamoAPI.BaseAggregator.Models;
using SocialDynamoAPI.BaseAggregator.ViewModels;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    public interface IPostSearch
    {
        Task<List<CompletePostVM>> GetPostDetailsAsync(List<Post> posts, string httpCookie);
    }
}
