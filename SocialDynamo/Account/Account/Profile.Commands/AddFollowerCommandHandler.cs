using Account.API.Infrastructure.Repositories;
using Account.Models.Users;
using MediatR;

namespace Account.API.Profile.Commands
{
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

        public async Task<bool> Handle(AddFollowerCommand addFollowerCommand, CancellationToken cancellationToken)
        {
            Follower follower = new()
            {
                FollowerId = addFollowerCommand.FollowerId,
                UserId = addFollowerCommand.UserId
            };

            await _followerRepository.AddFollower(follower);
            return true;
        }        
    }
}
