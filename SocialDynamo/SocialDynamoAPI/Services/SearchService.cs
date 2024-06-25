using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.Models;
using SocialDynamoAPI.BaseAggregator.ViewModels;
using System.Net.Http.Headers;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    //Aggregates searches across multiple microservices
    public class SearchService : ISearchService
    {
        private readonly HttpClient _client;
        private readonly ILogger<PostService> _logger;
        private readonly IPostSearch _postSearch;
        private readonly string _urlAddress = "https://api.socdyn.com:443";

        public SearchService(ILogger<PostService> logger, IPostSearch postSearch)
        {
            _client = new HttpClient();
            _logger = logger;
            _postSearch = postSearch;

            _client.DefaultRequestHeaders.Add("Origin", "https://socdyn.com");
        }

        /// <summary>
        /// Returns a fuzzy search on multiple microservices to find the closest value
        /// to the search term. 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<object> Search(string searchTerm, string httpCookie)
        {
            setHttpHeaderCookie(httpCookie);

            IEnumerable<UserDataVM> searchedUsers = await GetUsers(searchTerm);
            List<Post> searchedHashtags = (List<Post>)await GetPosts(searchTerm);

            List<CompletePostVM> completePosts = await _postSearch.GetPostDetailsAsync(searchedHashtags, httpCookie);

            var results = new
            {
                users = searchedUsers,
                posts = completePosts
            };
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
            string postsPath = _urlAddress + "/posts/fuzzy/" + searchTerm;

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
            string accountPath = _urlAddress + "/account/search/" + searchTerm;

            //Get followers from service
            var accountResponse = await _client.GetAsync(accountPath);

            if (accountResponse.IsSuccessStatusCode)
            { 
                users = await accountResponse.Content.ReadAsAsync<List<UserDataVM>>();
                _logger.LogInformation("----- Users searched for term in account microservice" +
                "Search term: {searchTerm}", searchTerm);
            }
            else _logger.LogInformation("----- Failed to find results for search term in  account microservice. " +
                "Search term: {searchTerm}", searchTerm);

            return users;
        }

        private void setHttpHeaderCookie(string httpCookie)
        {            
            _client.DefaultRequestHeaders.Authorization = null;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpCookie);
        }
    }
}
