using MediatR;

namespace Common.API.Profile.Commands
{
    public class AddFollowerCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public string FollowerId { get; set; }
    }
}
