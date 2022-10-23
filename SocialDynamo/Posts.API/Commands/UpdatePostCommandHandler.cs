using MediatR;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;
using Posts.Domain.Repositories;

namespace Posts.API.Commands
{
    //Command handler to update a specified post.
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public UpdatePostCommandHandler(IPostRepository postRepository, ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - updates the specified 
        /// post only if any of the information in the command differs from
        /// that stored in the DB. 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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

            _logger.LogInformation("----- Post updated with new information from command. Post: {@PostId}, " +
                "MediaItemIDs: {@MediaItemIds}, Hashtag: {@Hashtag}, Caption: {@Caption}", command.PostId,
                command.MediaItemIds, command.Hashtag, command.Caption);

            await _postRepository.UpdatePostAsync(post);
            return true;
        }
    }
}
