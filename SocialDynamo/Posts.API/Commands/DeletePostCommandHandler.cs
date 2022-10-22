using MediatR;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public DeletePostCommandHandler(IPostRepository postRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePostCommand command, CancellationToken cancellationToken)
        {
            await _postRepository.DeletePostAsync(command.PostId);
            return true;
        }
    }
}
