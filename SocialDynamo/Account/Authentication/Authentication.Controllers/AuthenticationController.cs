using Common.API.Commands;
using Common.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Common.API.Authentication.Authentication.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;

        }

        [HttpPut("register")]
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
                return ControllerExceptionHandler.HandleException(ex);
            }

        }

        [HttpPut("login")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            try
            {                
                var token = await _authenticationService.HandleCommandAsync(command, HttpContext);
                return new OkObjectResult(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpPut("logout")]
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
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpPut("refresh-token")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RefreshToken(RefreshJwtTokenCommand command)
        {
            try
            {
                var token = await _authenticationService.HandleCommandAsync(command, HttpContext);
                return new OkObjectResult(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }
    }
}
