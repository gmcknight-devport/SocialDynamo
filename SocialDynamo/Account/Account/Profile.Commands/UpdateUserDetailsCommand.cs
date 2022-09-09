using MediatR;

namespace Account.API.Profile.Commands
{
    public class UpdateUserDetailsCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
    }
}
