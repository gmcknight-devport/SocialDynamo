using Account.API.Infrastructure.Repositories;
using Account.Models.Users;
using MediatR;

namespace Account.API.Profile.Commands
{
    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserDetailsCommandHandler> _logger;

        public UpdateUserDetailsCommandHandler(IUserRepository userRepository, ILogger<UpdateUserDetailsCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateUserDetailsCommand updateUserDetailsCommand, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserAsync(updateUserDetailsCommand.UserId);

            user.Forename = updateUserDetailsCommand.Forename != null
                            ? updateUserDetailsCommand.Forename
                            : user.Forename;
            user.Surname = updateUserDetailsCommand.Surname != null
                            ? updateUserDetailsCommand.Surname
                            : user.Surname;

            await _userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}
