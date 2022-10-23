using MediatR;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    //Command handler to delete a specified post. 
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public DeletePostCommandHandler(IPostRepository postRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - deletes the specified post.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(DeletePostCommand command, CancellationToken cancellationToken)
        {
            await _postRepository.DeletePostAsync(command.PostId);
            _logger.LogInformation("----- Specified post deleted. Post: {@PostId", command.PostId);

            return true;
        }
    }
}
