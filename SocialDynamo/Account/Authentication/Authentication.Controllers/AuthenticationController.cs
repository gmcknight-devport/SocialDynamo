using Account.API.Commands;
using Account.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Account.API.Authentication.Authentication.Controllers
{    
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController : Controller
    {
        private readonly AuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(AuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            try
            {
                await _authenticationService.HandleCommandAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            try
            {
                var token = await _authenticationService.HandleCommandAsync(command);
                return new OkObjectResult(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Logout(LogoutUserCommand command)
        {
            try
            {
                await _authenticationService.HandleCommandAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RefreshToken(RefreshJwtTokenCommand command)
        {
            try
            {
                var token = await _authenticationService.HandleCommandAsync(command);
                return new OkObjectResult(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
