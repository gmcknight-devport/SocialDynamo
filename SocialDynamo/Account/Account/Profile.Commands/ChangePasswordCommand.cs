using MediatR;

namespace Account.API.Profile.Commands
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
