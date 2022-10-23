using MediatR;
using Posts.Domain.Models;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    //Command handler to add a comment. 
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public AddCommentCommandHandler(IPostRepository postRepository, 
                                        ICommentRepository commentRepository, 
                                        ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - creates a new comment for the 
        /// specified post and calls the relevant repository method.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(AddCommentCommand command, CancellationToken cancellationToken)
        {
            Post post = await _postRepository.GetPostAsync(command.PostId);
            Comment comment = new()
            {
                AuthorId = command.AuthorId,
                CommentText = command.Comment,
                Post = post,
                PostedAt = DateTime.UtcNow,
                Likes = new List<CommentLike>()
            };

            _logger.LogInformation("----- Comment added to post. Comment: {@Comment}, Post ID: {@PostId}"
                , command.Comment, command.PostId);

            await _commentRepository.AddCommentAsync(post.PostId, comment);
            return true;
        }
    }
}
