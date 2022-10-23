using Account.Domain.Repositories;
using Account.Models.Users;
using MediatR;

namespace Account.API.Profile.Commands
{
    //Command handler to change user's password when called. 
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(IUserRepository userRepository, ILogger<ChangePasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - calls relevant repository 
        /// method to change the password.
        /// </summary>
        /// <param name="changePasswordCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(ChangePasswordCommand changePasswordCommand, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserAsync(changePasswordCommand.UserId);
            user.Password = changePasswordCommand.Password;

            _logger.LogInformation("----- Changing user password - User: {@user}, " +
                "password information ommited for security", user);

            await _userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}
