using Media.API.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Common.Exceptions;

namespace Media.API.Controllers
{
    [ApiController]
    [Route("media")]
    [Authorize]
    public class MediaQueryController : ControllerBase
    {
        private readonly IMediaQueries _mediaQueries;
        private readonly ILogger<MediaQueryController> _logger;

        public MediaQueryController(IMediaQueries mediaQueries, ILogger<MediaQueryController> logger)
        {
            _mediaQueries = mediaQueries;
            _logger = logger;
        }

        [HttpGet]
        [Route("user/{userId}/{mediaItemId}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserPosts(string userId, string mediaItemId)
        {
            try
            {
                var blob = await _mediaQueries.GetBlob(userId, mediaItemId);
                return new OkObjectResult(blob);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }
    }
}
