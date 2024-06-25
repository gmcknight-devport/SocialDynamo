using MediatR;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    //Command handler - like a post. 
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<LikePostCommandHandler> _logger;

        public LikePostCommandHandler(IPostRepository postRepository, ILogger<LikePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - likes the specified post.  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(LikePostCommand command, CancellationToken cancellationToken)
        {
            await _postRepository.LikePostAsync(command.PostId, command.LikeUserId);
            _logger.LogInformation("----- Post liked by user. Post: {@PostId}, " +
               "User: {@UserId}", command.PostId, command.LikeUserId);

            return true;
        }
    }
}
