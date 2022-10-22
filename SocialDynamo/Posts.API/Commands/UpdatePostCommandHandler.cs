using MediatR;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public UpdatePostCommandHandler(IPostRepository postRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
        {
            Post post = await _postRepository.GetPostAsync(command.PostId);
            List<MediaItemId> mediaItems = post.MediaItemIds.ToList();

            if(command.MediaItemIds == null && command.Caption == null && command.Hashtag == null)
            {
                throw new ArgumentNullException("No new values provided");
            }

            post.MediaItemIds = command.MediaItemIds == null || command.MediaItemIds.All(mediaItems.Contains)
                            ? post.MediaItemIds
                            : command.MediaItemIds;

            post.Hashtag = command.Hashtag != null
                            ? command.Hashtag
                            : post.Hashtag;

            post.Caption = command.Caption != null
                            ? command.Caption
                            : post.Caption;

            await _postRepository.UpdatePostAsync(post);
            return true;
        }
    }
}
