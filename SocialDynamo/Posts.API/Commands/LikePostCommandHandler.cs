using MediatR;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public LikePostCommandHandler(IPostRepository postRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            await _postRepository.LikePostAsync(request.PostId, request.LikeUserId);
            return true;
        }
    }
}
