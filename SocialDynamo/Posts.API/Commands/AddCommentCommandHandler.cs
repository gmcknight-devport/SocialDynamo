using MediatR;
using Posts.Domain.Models;
using Posts.Infrastructure.Repositories;

namespace Posts.API.Commands
{
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

            await _commentRepository.AddCommentAsync(post.PostId, comment);
            return true;
        }
    }
}
