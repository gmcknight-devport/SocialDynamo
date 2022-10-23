using MediatR;
using Posts.Domain.Models;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    //Command handler to add a create a post. 
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public CreatePostCommandHandler(IPostRepository postRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - creates a new post with the 
        /// command information and calls the relevant repository method.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

            _logger.LogInformation("----- New post created with provided information for " +
                "user. User: {@AuthorId}", command.AuthorId);

            await _postRepository.CreatePostAsync(post);
            return true;
        }
    }
}
