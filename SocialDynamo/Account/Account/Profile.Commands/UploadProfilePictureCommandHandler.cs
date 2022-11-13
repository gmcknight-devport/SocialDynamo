using Account.Domain.Repositories;
using Account.Models.Users;
using ImageMagick;
using MediatR;

namespace Account.API.Profile.Commands
{
    //Command handler for UploadProfilePictureCommand.
    public class UploadProfilePictureCommandHandler : IRequestHandler<UploadProfilePictureCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UploadProfilePictureCommand> _logger;

        public UploadProfilePictureCommandHandler(IUserRepository userRepository, ILogger<UploadProfilePictureCommand> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles upload picture command. Checks user exists in repository, then converts the IFormfile
        /// to stream, compresses the stream using ImageMagick, adds profile picture to user, calls 
        /// repo method to update and save changes. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserAsync(request.UserId);

            using (var fileStream = request.ProfilePicture.OpenReadStream())
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                ImageOptimizer optimizer = new ImageOptimizer();
                optimizer.LosslessCompress(memoryStream);

                user.ProfilePicture = memoryStream.ToArray();
                await _userRepository.UpdateUserAsync(user);

                _logger.LogInformation("----- Upload proile picture command handled. User: {UserId}", request.UserId);
            }

            return true;
        }
    }
}
