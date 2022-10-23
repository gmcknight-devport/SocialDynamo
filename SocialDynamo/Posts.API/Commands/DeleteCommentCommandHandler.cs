using MediatR;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    //Command handler to delete a specified comment from a post. 
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public DeleteCommentCommandHandler(ICommentRepository commentRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - creates a new post with the 
        /// command information and calls the relevant repository method.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {
            await _commentRepository.DeleteCommentAsync(command.CommentId);
            _logger.LogInformation("----- Comment deleted. Commet: {@CommendId}", command.CommentId);

            return true;
        }
    }
}
