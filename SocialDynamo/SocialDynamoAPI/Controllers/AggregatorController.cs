using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialDynamoAPI.BaseAggregator.Services;
using SocialDynamoAPI.BaseAggregator.ViewModels;
using System.Net;
using Common.Exceptions;

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
                var httpOnlyCookie = Request.Cookies["token"];
                bool executed = await _postService.CreatePostAsync(createPostVM, httpOnlyCookie);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
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
                var httpOnlyCookie = Request.Cookies["token"];
                var searchResults = await _searchService.Search(searchTerm, httpOnlyCookie);
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
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
                var httpOnlyCookie = Request.Cookies["token"];
                var feed = await _postService.GetFeedAsync(userId, page, httpOnlyCookie);

                return Ok(feed);
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
                var httpOnlyCookie = Request.Cookies["token"];
                var userPosts = await _postService.GetUserPosts(userId, page, httpOnlyCookie);
                return Ok(userPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }
    }
}
