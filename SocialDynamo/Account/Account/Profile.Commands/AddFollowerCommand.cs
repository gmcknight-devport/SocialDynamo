using MediatR;

namespace Account.API.Profile.Commands
{
    public class AddFollowerCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public int FollowerId { get; set; }
    }
}
