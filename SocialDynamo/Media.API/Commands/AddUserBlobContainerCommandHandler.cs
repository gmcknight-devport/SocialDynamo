using Account.Tests.Exceptions;
using Azure.Storage.Blobs;
using MediatR;

namespace Media.API.Commands
{
    //Handles command - adds a user container azure storage. 
    public class AddUserBlobContainerCommandHandler : IRequestHandler<AddUserBlobContainerCommand, bool>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AddUserBlobContainerCommand> _logger;

        public AddUserBlobContainerCommandHandler(IConfiguration configuration, ILogger<AddUserBlobContainerCommand> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - adds a new container for the specified user 
        /// if one doesn't already exist. 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="DuplicateUserContainerException"></exception>
        public async Task<bool> Handle(AddUserBlobContainerCommand command, CancellationToken cancellationToken)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["ConnectionStrings:AzureStorage"]);
            var container = blobServiceClient.GetBlobContainerClient(command.UserId);

            if (container.Exists()) 
                throw new DuplicateUserContainerException("User container already exists, cannot create duplicate");

            _logger.LogInformation("----- User container added, User: {@UserId", command.UserId);

            await container.CreateAsync();
            return true;
        }
    }
}
