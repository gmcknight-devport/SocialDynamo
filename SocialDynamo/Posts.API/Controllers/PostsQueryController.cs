using Microsoft.AspNetCore.Mvc;
using Posts.API.Queries;
using System.Net;

namespace Posts.API.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostsQueryController : ControllerBase
    {
        private readonly IPostsQueries _postService;
        private readonly ILogger<PostsQueryController> _logger;

        public PostsQueryController(IPostsQueries postService, ILogger<PostsQueryController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet]
        [Route("user/{userId}/{page}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserPosts(string userId, int page)
        {
            try
            {
                var userPosts = await _postService.GetUserPostsAsync(userId, page);
                return new OkObjectResult(userPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("users/{page}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUsersPosts([FromQuery]List<string> userIds, int page)
        {
            try
            {
                var userPosts = await _postService.GetUsersPostsAsync(userIds, page);
                return new OkObjectResult(userPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("comments/{postId}/{page}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPostComments(Guid postId, int page)
        {
            try
            {
                var userPosts = await _postService.GetPostCommentsAsync(postId, page);
                return new OkObjectResult(userPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("postlikes/{postId}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPostLikes(Guid postId)
        {
            try
            {
                var userPosts = await _postService.GetPostLikesAsync(postId);
                return new OkObjectResult(userPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("commentlikes/{commentId}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetCommentLikes(Guid commentId)
        {
            try
            {
                var userPosts = await _postService.GetCommentLikesAsync(commentId);
                return new OkObjectResult(userPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
