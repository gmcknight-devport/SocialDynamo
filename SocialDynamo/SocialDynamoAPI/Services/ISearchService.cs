using Microsoft.AspNetCore.Mvc;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    public interface ISearchService
    {
        Task<object> Search(string searchTerm, string httpCookie);
    }
}