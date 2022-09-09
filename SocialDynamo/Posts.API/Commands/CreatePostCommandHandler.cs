using MediatR;
using Posts.Domain.Models;
using Posts.Infrastructure.Repositories;

namespace Posts.API.Commands
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public CreatePostCommandHandler(IPostRepository postRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(CreatePostCommand command, CancellationToken cancellationToken)
        {
            string hashtag = string.Empty;

            if (command.Hashtag != null)
                hashtag = command.Hashtag;

            Post post = new()
            {
                AuthorId = command.AuthorId,
                Hashtag = hashtag,
                Caption = command.Caption,
                MediaItemIds = command.MediaItemIds,
                PostedAt = DateTime.UtcNow,
                Likes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            await _postRepository.CreatePostAsync(post);
            return true;
        }
    }
}
