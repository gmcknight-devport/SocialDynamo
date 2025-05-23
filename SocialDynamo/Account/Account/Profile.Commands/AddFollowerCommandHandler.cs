﻿using Common.Domain.Repositories;
using Common.Models.Users;
using MediatR;

namespace Common.API.Profile.Commands
{
    //Command handler to add a follower when called. 
    public class AddFollowerCommandHandler : IRequestHandler<AddFollowerCommand, bool>
    {
        private readonly IFollowerRepository _followerRepository;
        private readonly ILogger<AddFollowerCommandHandler> _logger;

        public AddFollowerCommandHandler(IFollowerRepository followerRepository, 
                                        ILogger<AddFollowerCommandHandler> logger)
        {
            _followerRepository = followerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - creates a new follower and calls relevant repository method add to user.
        /// </summary>
        /// <param name="addFollowerCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(AddFollowerCommand addFollowerCommand, CancellationToken cancellationToken)
        {
            Follower follower = new()
            {
                FollowerId = addFollowerCommand.FollowerId
            };

            _logger.LogInformation("----- Add user follower - Follower: {@FollowerId}", follower.FollowerId);

            await _followerRepository.AddFollower(addFollowerCommand.UserId, follower);
            return true;
        }        
    }
}
