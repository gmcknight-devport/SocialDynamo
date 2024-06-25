using Common.API.Profile.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Common.Exceptions;

namespace Common.API.Controllers
{
    [ApiController]
    [Route("account")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly Mediator _mediator;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(Mediator mediator, ILogger<ProfileController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPut("addfollower")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(AddFollowerCommand command)
        {
            try
            {
                bool executed = await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpPut("changepassword")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(ChangePasswordCommand command)
        {
            try
            {
                bool executed = await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpPut("updateprofiledescription")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(UpdateProfileDescriptionCommand command)
        {
            try
            {
                bool executed = await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpPut("updateuserdetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(UpdateUserDetailsCommand command)
        {
            try
            {
                bool executed = await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpPut("uploadprofilepicture")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(UploadProfilePictureCommand command)
        {
            try
            {
                bool executed = await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
                //return BadRequest(ex.Message);
            }
        }
    }
}
