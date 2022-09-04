using MediatR;

namespace Account.API.Profile.Commands
{
    public class UpdateUserDetailsCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
    }
}
