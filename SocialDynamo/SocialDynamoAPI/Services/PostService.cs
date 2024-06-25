using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.Models;
using SocialDynamoAPI.BaseAggregator.ValueObjects;
using SocialDynamoAPI.BaseAggregator.ViewModels;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    //Aggregates post requests involving multiple microservices. 
    public class PostService : IPostService, IPostSearch
    {
        private readonly HttpClient _client;
        private readonly ILogger<PostService> _logger;
        private readonly string _urlAddress = "https://api.socdyn.com:443";

        public PostService(ILogger<PostService> logger)
        {
            _client = new HttpClient();
            _logger = logger;

            _client.DefaultRequestHeaders.Add("Origin", "https://socdyn.com");
        }

        /// <summary>
        /// Create a new post in the Post microservice returning the mediaItemIds to
        /// send to the Media microservice to create a new blob
        /// </summary>
        /// <param name="createPostVm"></param>
        /// <param name="httpCookie"></param>
        /// <returns></returns>
        public async Task<bool> CreatePostAsync(CreatePostVM createPostVM, string httpCookie)
        {
            List<MediaItemId> mediaItemIds = new();
            int index = 0;

            //Create MediaItemIds
            foreach (var m in createPostVM.Files)
            {
                mediaItemIds.Add(MediaItemId.Create(createPostVM.AuthorId, index));
                index++;
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
                    //Set cookie header
                    setHttpHeaderCookie(httpCookie);

                    // Create a stream content from the file's stream
                    var fileContent = new StreamContent(createPostVM.Files[i].OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(createPostVM.Files[i].ContentType);

                    var formContent = new MultipartFormDataContent
                    {
                        { new StringContent(createPostVM.AuthorId), "UserId" },
                        { new StringContent(mediaItemIds.ElementAt(i).Id), "MediaItemId" },
                        { fileContent, "File", createPostVM.Files[i].FileName }
                    };

                    //Call API
                    var mediaResponse = await _client.PutAsync(_urlAddress + "/media/upload", formContent);
                    mediaResponse.EnsureSuccessStatusCode();

                    _logger.LogInformation("----- MediaVm created and uploaded via media microservice");
                }
                catch (Exception e)
                {                    
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

            //Set cookie header for api request
            setHttpHeaderCookie(httpCookie);
            
            //Request
            var postsResponse = await _client.PutAsJsonAsync(_urlAddress + "/posts/post", postDetailsVM);
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
        /// <param name="page"></param>
        /// <param name="httpCookie"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetFeedAsync(string userId, int page, string httpCookie)
        {
            List<CompletePostVM> completePostVMs = new();

            //Set cookie header
            setHttpHeaderCookie(httpCookie);

            var following = await GetFollowing(userId);
            var postsDetails = await GetPosts(following, page);
            completePostVMs = await GetPostDetailsAsync(postsDetails, httpCookie);

            return new OkObjectResult(completePostVMs);
        }

        /// <summary>
        /// Return the posts for a specific user. Calls post microservice to get posts
        /// for a user for a specified page. Calls media microservice to get the media data. 
        /// Aggregates and returns complete posts. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="httpCookie"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<object> GetUserPosts(string userId, int page, string httpCookie)
        {           
            List<CompletePostVM> completePostVMs = new();
            List<Post> postsDetails = new();

            string postsPath =_urlAddress + "/posts/user/" + userId + "/" + page.ToString();
            setHttpHeaderCookie(httpCookie);
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
                List<Uri> postMediaData = GetPostMedia(post).Result;
                completePostVMs.Add(new CompletePostVM(post, userData, postMediaData));
            }

            _logger.LogInformation("----- Media data received from media microservice");

            return completePostVMs;            
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
            string accountPath = _urlAddress + "/account/Profile/" + userId;

            //Get user data from service
            var accountResponse = await _client.GetAsync(accountPath);

            if (!accountResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException("User data request failed");
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
            string accountPath = _urlAddress + "/account/following/" + userId;

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
            string postsPath = _urlAddress + "/posts/users/" + page.ToString();

            //Get post details from microservice.
            //If microservice fails, logs failure but allows original call to continue as other services 
            //could run successfully and return some data to the client. 
            var postsResponse = await _client.PostAsJsonAsync(postsPath, users);

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
        private async Task<List<Uri>> GetPostMedia(Post post)
        {
            List<Uri> blobData = new();
            string mediaPath = _urlAddress + "/media/user/";

            foreach (MediaItemId id in post.MediaItemIds)
            {
                var newId = id.Id.ToString().Replace("/", "%2F").Replace(":", "%3A");
                var callPath = mediaPath + post.AuthorId + "/" + newId;

                var mediaResponse = await _client.GetAsync(callPath);

                //If microservice fails, logs failure but allows original call to continue as other services 
                //could run successfully and return some data to the client. 
                if (mediaResponse.IsSuccessStatusCode)
                {
                    blobData.Add(await mediaResponse.Content.ReadAsAsync<Uri>());
                    _logger.LogInformation("----- Media data received from media microservice");
                }
                else _logger.LogInformation("----- Failed to get media data for post from microservice. " +
                    "Post: {PostId}", post.PostId);
            }
            return blobData;
        }

        public async Task<List<CompletePostVM>> GetPostDetailsAsync(List<Post> posts, string httpCookie)
        {
            List<CompletePostVM> completePostVMs = new();
            setHttpHeaderCookie(httpCookie);

            foreach (Post post in posts)
            {
                var userData = await GetUserData(post.AuthorId);
                List<Uri> postMediaData = GetPostMedia(post).Result;
                completePostVMs.Add(new CompletePostVM(post, userData, postMediaData));
            }

            return completePostVMs;
        }

        private void setHttpHeaderCookie(string httpCookie)
        {
            _client.DefaultRequestHeaders.Authorization = null;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpCookie);
        }
    }
}
