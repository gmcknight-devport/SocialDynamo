using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.Models;
using SocialDynamoAPI.BaseAggregator.ViewModels;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    //Aggregates searches across multiple microservices
    public class SearchService : ISearchService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PostService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Task<string?>? _authorisationToken;

        public SearchService(IConfiguration configuration, ILogger<PostService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            //Get stored bearer token and add to authorisation header of client
            _authorisationToken = _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authorisationToken.Result);
        }

        /// <summary>
        /// Returns a fuzzy search on multiple microservices to find the closest value
        /// to the search term. 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IActionResult>> Search(string searchTerm)
        {
            List<OkObjectResult> results = new();

            var searchedUsers = await GetUsers(searchTerm);
            var searchedHashtags = await GetPosts(searchTerm);

            results.Add(new OkObjectResult(searchedUsers));
            results.Add(new OkObjectResult(searchedHashtags));

            return results;
        }

        /// <summary>
        /// Private method to execute fuzzy search on the posts microservice for posts with a 
        /// specified hashtag and return results. 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        private async Task<IEnumerable<Post>> GetPosts(string searchTerm)
        {
            List<Post> postsDetails = new();
            string postsPath = _configuration["Service:Posts"]+ "/posts/fuzzy/" + searchTerm;

            //Get post details from microservice.
            //If microservice fails, logs failure but allows original call to continue as other services 
            //could run successfully and return some data to the client. 
            var postsResponse = await _client.GetAsync(postsPath);
            if (postsResponse.IsSuccessStatusCode)
            {
                postsDetails = await postsResponse.Content.ReadAsAsync<List<Post>>();
                _logger.LogInformation("----- Posts searched for term in posts microservice" +
                "Search term: {searchTerm}", searchTerm);
            }
            else _logger.LogInformation("----- Failed to find results for search term in posts microservice. " +
                "Search term: {searchTerm}", searchTerm);

            return postsDetails;
        }

        /// <summary>
        /// Private method to execute fuzzy search on the account microservice for users
        /// and return results. 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        private async Task<IEnumerable<UserDataVM>> GetUsers(string searchTerm)
        {
            List<UserDataVM> users = new();
            string accountPath = "http://host.docker.internal:8080/account/search/" + searchTerm;

            //Get followers from service
            var accountResponse = await _client.GetAsync(accountPath);

            if (accountResponse.IsSuccessStatusCode)
            {
                throw new Exception(accountResponse.Content.ReadAsStringAsync().Result);
                users = await accountResponse.Content.ReadAsAsync<List<UserDataVM>>();
                _logger.LogInformation("----- Users searched for term in account microservice" +
                "Search term: {searchTerm}", searchTerm);
            }
            else _logger.LogInformation("----- Failed to find results for search term in  account microservice. " +
                "Search term: {searchTerm}", searchTerm);

            return users;
        }
    }
}
