using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.Models;
using SocialDynamoAPI.BaseAggregator.ValueObjects;
using SocialDynamoAPI.BaseAggregator.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Text;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    //Aggregates post requests involving multiple microservices. 
    public class PostService : IPostService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PostService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Task<string?>? _authorisationToken;

        public PostService(IConfiguration configuration, ILogger<PostService> logger, IHttpContextAccessor httpContextAccessor)
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
        /// Create a new post in the Post microservice returning the mediaItemIds to
        /// send to the Media microservice to create a new blob
        /// </summary>
        /// <param name="mediaItemVm"></param>
        /// <param name="postDetailsVM"></param>
        /// <returns></returns>
        public async Task<bool> CreatePostAsync(CreatePostVM createPostVM)
        {
            List<MediaItemId> mediaItemIds = new();

            //Create MediaItemIds
            foreach (var m in createPostVM.Files)
            {
                mediaItemIds.Add(MediaItemId.Create(createPostVM.AuthorId));
            }

            //Validate
            if (mediaItemIds == null)
                throw new ArgumentNullException(nameof(mediaItemIds));
            else if (mediaItemIds.Distinct().Count() != mediaItemIds.Count())
                throw new ArgumentException("Media Item Ids are not unique");

            //Loop through number of files
            for (int i = 0; i < createPostVM.Files.Count; i++)
            {
                //Try to upload to media API
                try
                {
                    HttpResponseMessage mediaResponse = new();

                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(createPostVM.AuthorId), "UserId");
                        content.Add(new StringContent(mediaItemIds.ElementAt(i).Id), "MediaItemId");
                        content.Add(new StringContent(mediaItemIds.ElementAt(i).Id), "MediaItemId");

                        var fileName = createPostVM.Files.ElementAt(i).FileName;
                        content.Add(new StreamContent(createPostVM.Files.ElementAt(i).OpenReadStream()), "File", fileName);

                        mediaResponse = await _client.PutAsync("http://host.docker.internal:8081" + "/media/upload", content);
                        mediaResponse.EnsureSuccessStatusCode();

                        _logger.LogInformation("----- MediaVm created and uploaded via media microservice");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation("----- MediaVm failed to upload");
                    return false;
                }
            }

            //Create PostDetailsVM and upload
            PostDetailsVM postDetailsVM = new()
            {
                AuthorId = createPostVM.AuthorId,
                Caption = createPostVM.Caption,
                Hashtag = createPostVM.Hashtag,
                MediaItemIds = mediaItemIds
            };
            
            var postsResponse = await _client.PutAsync("http://host.docker.internal:8082" + "/posts/post", 
                                            new StringContent(JsonConvert.SerializeObject(postDetailsVM), 
                                            Encoding.UTF8, "application/json"));
            postsResponse.EnsureSuccessStatusCode();


            _logger.LogInformation("----- Post details created and uploaded via posts microservice");
            
            return true;
        }

        /// <summary>
        /// Get posts for user feed based. Queries account service to get a list
        /// of followers, then queries the posts service to get most recent posts from them by
        /// page. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetFeedAsync(string userId, int page)
        {
            List<CompletePostVM> completePostVMs = new();

            var following = await GetFollowing(userId);
            var userData = await GetUserData(userId);
            var postsDetails = await GetPosts(following, page);

            foreach (Post post in postsDetails)
            {
                List<byte[]> postMediaData = GetPostMedia(post).Result;
                completePostVMs.Add(new CompletePostVM(post, userData, postMediaData));
            }

            return new OkObjectResult(completePostVMs);
        }

        /// <summary>
        /// Return the posts for a specific user. Calls post microservice to get posts
        /// for a user for a specified page. Calls media microservice to get the media data. 
        /// Aggregates and returns complete posts. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<IActionResult> GetUserPosts(string userId, int page)
        {           
            List<CompletePostVM> completePostVMs = new();
            List<Post> postsDetails = new();
            //string postsPath = _configuration["Service:Posts"] + "/posts/user/" + userId + "/" + page.ToString();
            string postsPath = "http://host.docker.internal:8082" + "/posts/user/" + userId + "/" + page.ToString();
            var userData = await GetUserData(userId);

            //Get post details from microservice
            var postsResponse = await _client.GetAsync(postsPath);

            if (postsResponse.IsSuccessStatusCode)
            {
                postsDetails = await postsResponse.Content.ReadAsAsync<List<Post>>();
            }
            else throw new HttpRequestException("Post request failed");

            _logger.LogInformation("----- User data received from posts microservice");


            foreach (Post post in postsDetails)
            {
                List<byte[]> postMediaData = GetPostMedia(post).Result;
                completePostVMs.Add(new CompletePostVM(post, userData, postMediaData));
            }

            return new OkObjectResult(completePostVMs);
            
        }

        /// <summary>
        /// Private method return follower data from the account microservice. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<UserDataVM> GetUserData(string userId)
        {
            UserDataVM userData;
            //string accountPath = _configuration["Service:Account"] + "/account/Profile/" + userId;
            string accountPath = "http://host.docker.internal:8080" + "/account/Profile/" + userId;

            //Get user data from service
            var accountResponse = await _client.GetAsync(accountPath);
            if (!accountResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Follower request failed");
            }

            userData = await accountResponse.Content.ReadAsAsync<UserDataVM>();
            _logger.LogInformation("----- User data received from account microservice");

            return userData;
        }

        /// <summary>
        /// Private method return follower data from the account microservice. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<List<UserDataVM>> GetFollowing(string userId)
        {
            List<UserDataVM> following;
            //string accountPath = _configuration["Service:Account"] + "/account/following/" + userId;
            string accountPath = "http://host.docker.internal:8080" + "/account/following/" + userId;

            //Get followers from service
            var accountResponse = await _client.GetAsync(accountPath);
            if (!accountResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Follower request failed");
            }

            following = await accountResponse.Content.ReadAsAsync<List<UserDataVM>>();
            _logger.LogInformation("----- User following data received from account microservice");

            return following;
        }

        /// <summary>
        /// Private method returning post data based on parameters from the posts microservice. 
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<List<Post>> GetPosts(List<UserDataVM> userIds, int page)
        {
            List<Post> postsDetails = new();
            string[] users = userIds.Select(u => u.UserId).ToArray();
            string postsPath = _configuration["Service:Posts"] + "/posts/users/" + page.ToString();

            //Get post details from microservice.
            //If microservice fails, logs failure but allows original call to continue as other services 
            //could run successfully and return some data to the client. 
            var postsResponse = await _client.PostAsync(postsPath, new StringContent(JsonConvert.SerializeObject(users),
                                            Encoding.UTF8, "application/json"));

            if (postsResponse.IsSuccessStatusCode)
            {
                postsDetails = await postsResponse.Content.ReadAsAsync<List<Post>>();
                _logger.LogInformation("----- Feed posts data received from posts microservice");
            }
            else _logger.LogInformation("----- Failed to get post details from microservice for users");

            return postsDetails;
        }

        /// <summary>
        /// Private method returning data from the media microservice for the specified post.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<List<byte[]>> GetPostMedia(Post post)
        {
            List<byte[]> blobData = new();
            //string mediaPath = _configuration["Service:Media"] + "/media/user/";
            string mediaPath = "http://host.docker.internal:8081" + "/media/user/";

            foreach (MediaItemId id in post.MediaItemIds)
            {
                var newId = id.Id.ToString().Replace("/", "%2F");
                newId = newId.Replace(":", "%3a");
                var callPath = mediaPath + post.AuthorId + "/" + newId;

                var mediaResponse = await _client.GetAsync(callPath);

                //If microservice fails, logs failure but allows original call to continue as other services 
                //could run successfully and return some data to the client. 
                if (mediaResponse.IsSuccessStatusCode)
                {
                    blobData.Add(await mediaResponse.Content.ReadAsAsync<byte[]>());
                }
                else _logger.LogInformation("----- Failed to get media data for post from microservice. " +
                    "Post: {PostId}", post.PostId);
            }
            return blobData;
        }
    }
}
