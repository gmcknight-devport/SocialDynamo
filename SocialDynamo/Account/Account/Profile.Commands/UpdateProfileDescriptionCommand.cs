using MediatR;

namespace Account.API.Profile.Commands
{
    public class UpdateProfileDescriptionCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public string Description { get; set; }
    }
}
