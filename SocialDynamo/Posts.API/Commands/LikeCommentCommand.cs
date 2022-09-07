using MediatR;

namespace Posts.API.Commands
{
    public class LikeCommentCommand : IRequest<bool>
    {
        public Guid CommentId { get; set; }
        public int LikeUserId { get; set; }
    }
}
