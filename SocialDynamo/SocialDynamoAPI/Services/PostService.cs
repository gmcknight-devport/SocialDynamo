using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.Models;
using SocialDynamoAPI.BaseAggregator.ValueObjects;
using SocialDynamoAPI.BaseAggregator.ViewModels;

namespace SocialDynamoAPI.BaseAggregator.Services
{
    //Aggregates post requests involving multiple microservices. 
    public class PostService : IPostService
    {
        private readonly HttpClient _client;
        private readonly ILogger<PostService> _logger;

        public PostService(ILogger<PostService> logger)
        {
            _client = new HttpClient();
            _logger = logger;
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
            {
                throw new ArgumentNullException(nameof(mediaItemIds));

            }
            else if (mediaItemIds.Distinct().Count() != mediaItemIds.Count())
            {
                throw new ArgumentException("Media Item Ids are not unique");
            }

            //Create MediaItemVMs and call to upload
            for (int i = 0; i < createPostVM.Files.Count; i++)
            {
                MediaItemVM temp = new()
                {
                    UserId = createPostVM.AuthorId,
                    MediaItemId = mediaItemIds.ElementAt(i).ToString(),
                    File = createPostVM.Files.ElementAt(i)
                };

                //Call upload api method here
                var mediaResponse = ApiPostCall("https://media.api/media", temp);
                mediaResponse.Result.EnsureSuccessStatusCode();
            }

            _logger.LogInformation("----- MediaVms created and uploaded via media microservice");

            //Create PostDetailsVM and upload
            PostDetailsVM postDetailsVM = new()
            {
                AuthorId = createPostVM.AuthorId,
                Caption = createPostVM.Caption,
                Hashtag = createPostVM.Hashtag,
                MediaItemIds = mediaItemIds
            };

            //Call createposthere api method
            var postsResponse = ApiPostCall("https://posts.api/posts", postDetailsVM);

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
            var postsDetails = await GetPosts(following, page);

            foreach (Post post in postsDetails)
            {
                List<BinaryData> postMediaData = GetPostMedia(post).Result;
                completePostVMs.Add(new CompletePostVM(post, postMediaData));
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
            string postsPath = "https://posts.api/posts/user/" + userId + "/" + page.ToString();

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
                List<BinaryData> postMediaData = GetPostMedia(post).Result;
                completePostVMs.Add(new CompletePostVM(post, postMediaData));
            }

            return new OkObjectResult(completePostVMs);
        }

        /// <summary>
        /// Private method return follower data from the account microservice. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<List<string>> GetFollowing(string userId)
        {
            List<string> following;
            string accountPath = "https://account.api/account/Followers/" + userId;

            //Get followers from service
            var accountResponse = await _client.GetAsync(accountPath);
            if (!accountResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Follower request failed");
            }

            following = await accountResponse.Content.ReadAsAsync<List<string>>();

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
        private async Task<List<Post>> GetPosts(List<string> userIds, int page)
        {
            List<Post> postsDetails = new();
            string postsPath = "https://posts.api/posts/users/" + page.ToString() + "?";

            //Prepare url for post request
            foreach (var s in userIds)
            {
                postsPath += "userIds=" + s;
            }

            //Get post details from microservice.
            //If microservice fails, logs failure but allows original call to continue as other services 
            //could run successfully and return some data to the client. 
            var postsResponse = await _client.GetAsync(postsPath);
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
        private async Task<List<BinaryData>> GetPostMedia(Post post)
        {
            List<BinaryData> blobData = new();
            List<CompletePostVM> completePosts = new();
            string mediaPath = "https://media.api/user/";

            foreach (MediaItemId id in post.MediaItemIds)
            {
                var callPath = mediaPath + post.AuthorId + "/" + id.ToString();
                var mediaResponse = await _client.GetAsync(mediaPath);

                //If microservice fails, logs failure but allows original call to continue as other services 
                //could run successfully and return some data to the client. 
                if (mediaResponse.IsSuccessStatusCode)
                {
                    blobData.Add(await mediaResponse.Content.ReadAsAsync<BinaryData>());
                }
                else _logger.LogInformation("----- Failed to get media data for post from microservice. " +
                    "Post: {PostId}", post.PostId);
            }

            return blobData;
        }

        /// <summary>
        /// Private helper method to call the http client and return a message
        /// based on the parameter values. 
        /// </summary>
        /// <param name="apiPath"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> ApiPostCall(string apiPath, object jsonObject)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(apiPath, jsonObject);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
