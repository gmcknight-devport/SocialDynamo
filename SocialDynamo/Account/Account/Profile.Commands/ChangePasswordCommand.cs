using MediatR;

namespace Account.API.Profile.Commands
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }
}
