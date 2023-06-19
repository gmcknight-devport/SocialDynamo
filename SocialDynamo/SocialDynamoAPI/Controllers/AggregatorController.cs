using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.Services;
using SocialDynamoAPI.BaseAggregator.ViewModels;
using System.Net;

namespace SocialDynamoAPI.BaseAggregator.Controllers
{
    [ApiController]
    [Route("baseaggregate")]
    [Authorize]
    public class AggregatorController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ISearchService _searchService;
        private readonly ILogger<AggregatorController> _logger;

        public AggregatorController(IPostService postService, ISearchService searchService, ILogger<AggregatorController> logger)
        {
            _postService = postService;
            _searchService = searchService;
            _logger = logger;
        }

        [HttpPut]
        [Route("post")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreatePost([FromForm]CreatePostVM createPostVM)
        {
            try
            {
                bool executed = await _postService.CreatePostAsync(createPostVM);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("search/{searchTerm}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FuzzySearch(string searchTerm)
        {
            try
            {
                var searchResults = await _searchService.Search(searchTerm);
                return new OkObjectResult(searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("feed/{userId}/{page}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFeed(string userId, int page)
        {
            try
            {
                var feed = await _postService.GetFeedAsync(userId, page);
                return new JsonResult(feed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("userposts/{userId}/{page}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserPosts(string userId, int page)
        {
            try
            {
                var userPosts = await _postService.GetUserPosts(userId, page);
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
