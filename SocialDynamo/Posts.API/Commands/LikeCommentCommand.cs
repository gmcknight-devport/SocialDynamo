using MediatR;

namespace Posts.API.Commands
{
    public class LikeCommentCommand : IRequest<bool>
    {
        public Guid CommentId { get; set; }
        public string LikeUserId { get; set; }
    }
}
