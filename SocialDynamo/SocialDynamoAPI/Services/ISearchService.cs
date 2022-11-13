using Microsoft.AspNetCore.Mvc;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<IActionResult>> Search(string searchTerm);
    }
}