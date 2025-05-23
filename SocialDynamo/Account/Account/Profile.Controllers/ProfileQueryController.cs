﻿using Common.API.Profile.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Common.Exceptions;

namespace Common.API.Account.Profile.Controllers
{
    [ApiController]
    [Route("account")]
    [Authorize]
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
        [Route("Profile/{userId}")]
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
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpPut]
        [Route("Profiles")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetProfileInformation(List<string> userIds)
        {
            try
            {
                var profileInformation = await _queryService.GetProfileInformation(userIds);
                return Ok(profileInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpGet]
        [Route("followers/{userId}")]
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
                return ControllerExceptionHandler.HandleException(ex);
            }
        }

        [HttpGet]
        [Route("following/{userId}")]
        [ProducesResponseType(typeof(OkObjectResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserFollowing(string userId)
        {
            try
            {
                var followerCollection = await _queryService.GetUserFollowing(userId);
                return new OkObjectResult(followerCollection);
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
                var searchedUsers = await _queryService.SearchUser(searchTerm);
                return new OkObjectResult(searchedUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ControllerExceptionHandler.HandleException(ex);
            }
        }
    }
}
