using Azure.Storage.Blobs;
using Media.API.Exceptions;
using MediatR;

namespace Media.API.Commands
{
    //Command handler to delete a user container. 
    public class DeleteUserContainerCommandHandler : IRequestHandler<DeleteUserContainerCommand, bool>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeleteUserContainerCommandHandler> _logger;

        public DeleteUserContainerCommandHandler(IConfiguration configuration, 
                                                 ILogger<DeleteUserContainerCommandHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - deletes the specified user container 
        /// if it exists
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NoUserContainerException"></exception>
        public async Task<bool> Handle(DeleteUserContainerCommand command, CancellationToken cancellationToken)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["ConnectionStrings:AzureStorage"]);
            var container = blobServiceClient.GetBlobContainerClient(command.UserId);

            if (!container.Exists())
                throw new NoUserContainerException("No user container to delete");

            _logger.LogInformation("----- Container of user deleted, User: {@UserId}", command.UserId);

            await container.DeleteAsync();

            return true;
        }
    }
}
