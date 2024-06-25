using Common.Domain.Repositories;
using Common.Models.Users;
using MediatR;

namespace Common.API.Profile.Commands
{
    //Command handler to update specified profile desciption when called. 
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

        /// <summary>
        /// Handle method of mediatr interface - calls relevant repository 
        /// method to complete the update to profile description.
        /// </summary>
        /// <param name="updateProfileDescriptionCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(UpdateProfileDescriptionCommand updateProfileDescriptionCommand, 
                                            CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserAsync(updateProfileDescriptionCommand.UserId);
            user.ProfileDescription = updateProfileDescriptionCommand.Description;

            _logger.LogInformation("----- Updating profile description - User: {@user}, " +
                "New description: {@Description}", user, updateProfileDescriptionCommand.Description);

            await _userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}
