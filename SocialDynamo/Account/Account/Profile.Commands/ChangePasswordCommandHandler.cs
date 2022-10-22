using Account.Domain.Repositories;
using Account.Models.Users;
using MediatR;

namespace Account.API.Profile.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(IUserRepository userRepository, ILogger<ChangePasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangePasswordCommand changePasswordCommand, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserAsync(changePasswordCommand.UserId);
            user.Password = changePasswordCommand.Password;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}
