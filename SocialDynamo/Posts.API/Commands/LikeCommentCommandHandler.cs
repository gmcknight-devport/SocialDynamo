using MediatR;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    public class LikeCommentCommandHandler : IRequestHandler<LikeCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public LikeCommentCommandHandler(ICommentRepository commentRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(LikeCommentCommand command, CancellationToken cancellationToken)
        {
            await _commentRepository.LikeCommentAsync(command.CommentId, command.LikeUserId);
            return true;
        }
    }
}
