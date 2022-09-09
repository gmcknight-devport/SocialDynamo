using Account.API.Profile.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Account.API.Account.Profile.Controllers
{
    [ApiController]
    [Route("[account]")]
    public class ProfileQueryController : Controller
    {

        private readonly IProfileQueries _queryService;
        private readonly ILogger<ProfileQueryController> _logger;

        public ProfileQueryController(IProfileQueries queryService, ILogger<ProfileQueryController> logger)
        {
            _queryService = queryService;
            _logger = logger;   
        }

        [HttpGet]
        [Route("[Profile]/{userId}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetProfileInformation(string userId)
        {
            try
            {
                var profileInformation = await _queryService.GetProfileInformation(userId);
                return new OkObjectResult(profileInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[Followers]/{userId}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserFollowers(string userId)
        {
            try
            {
                var followerCollection = await _queryService.GetUserFollowers(userId);
                return new OkObjectResult(followerCollection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[Following]/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<OkObjectResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IEnumerable<IActionResult>> GetUserFollowing(string userId)
        {
            try
            {
                IEnumerable<OkObjectResult> followerCollection = 
                    (IEnumerable<OkObjectResult>)await _queryService.GetUserFollowing(userId);

                return followerCollection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                IEnumerable<BadRequestObjectResult> badRequest = new List<BadRequestObjectResult>()
                {
                    new BadRequestObjectResult(ex.Message)
                };
                
                return badRequest;
            }
        }
    }
}
