using Account.API.Infrastructure.Repositories;
using Account.Models.Users;
using MediatR;

namespace Account.API.Profile.Commands
{
    public class UpdateProfileDescriptionCommandHandler : IRequestHandler<UpdateProfileDescriptionCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateProfileDescriptionCommandHandler> _logger;

        public UpdateProfileDescriptionCommandHandler(IUserRepository userRepository, 
                                                     ILogger<UpdateProfileDescriptionCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateProfileDescriptionCommand updateProfileDescriptionCommand, 
                                            CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserAsync(updateProfileDescriptionCommand.UserId);
            user.ProfileDescription = updateProfileDescriptionCommand.Description;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}
