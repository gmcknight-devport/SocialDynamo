using Account.Domain.Repositories;
using Account.Models.Users;
using MediatR;

namespace Account.API.Profile.Commands
{
    //Command handler to update user details when called. 
    //Updates forename and/or surname
    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserDetailsCommandHandler> _logger;

        public UpdateUserDetailsCommandHandler(IUserRepository userRepository, ILogger<UpdateUserDetailsCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - calls relevant repository 
        /// method to complete the user details update.
        /// </summary>
        /// <param name="updateUserDetailsCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(UpdateUserDetailsCommand updateUserDetailsCommand, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserAsync(updateUserDetailsCommand.UserId);

            user.Forename = updateUserDetailsCommand.Forename != null
                            ? updateUserDetailsCommand.Forename
                            : user.Forename;
            user.Surname = updateUserDetailsCommand.Surname != null
                            ? updateUserDetailsCommand.Surname
                            : user.Surname;

            _logger.LogInformation("----- Updating user details - User: {@user}", user);

            await _userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}
