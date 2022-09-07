using MediatR;
using Posts.Infrastructure.Repositories;

namespace Posts.API.Commands
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public DeleteCommentCommandHandler(ICommentRepository commentRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {
            await _commentRepository.DeleteCommentAsync(command.CommentId);
            return true;
        }
    }
}
