using MediatR;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    //Command handler - like a comment. 
    public class LikeCommentCommandHandler : IRequestHandler<LikeCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public LikeCommentCommandHandler(ICommentRepository commentRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - likes the specified comment. 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(LikeCommentCommand command, CancellationToken cancellationToken)
        {
            await _commentRepository.LikeCommentAsync(command.CommentId, command.LikeUserId);
            _logger.LogInformation("----- Comment liked by user. Comment: {@CommentId}, " +
                "User: {@UserId}", command.CommentId, command.LikeUserId);

            return true;
        }
    }
}
