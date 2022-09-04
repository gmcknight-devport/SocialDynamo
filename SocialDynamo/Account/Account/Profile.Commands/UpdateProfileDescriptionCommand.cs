using MediatR;

namespace Account.API.Profile.Commands
{
    public class UpdateProfileDescriptionCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public string Description { get; set; }
    }
}
