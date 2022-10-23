using Azure.Storage.Blobs;
using Media.API.Exceptions;
using MediatR;

namespace Media.API.Commands
{
    //Handles command - deleted a specified blob from the user container. 
    public class DeleteBlobCommandHandler : IRequestHandler<DeleteBlobCommand, bool>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeleteBlobCommandHandler> _logger;

        public DeleteBlobCommandHandler(IConfiguration configuration, ILogger<DeleteBlobCommandHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - deletes the specified blob of the 
        /// specified user if it exists
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NoUserContainerException"></exception>
        public async Task<bool> Handle(DeleteBlobCommand command, CancellationToken cancellationToken)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["ConnectionStrings:AzureStorage"]);
            var container = blobServiceClient.GetBlobContainerClient(command.UserId);
            

            if (!container.Exists())
                throw new NoUserContainerException("No user container to find blob to delete");

            _logger.LogInformation("----- Container has been found, deleting blob if it exists");

            await container.DeleteBlobIfExistsAsync(command.MediaItemId);

            return true;
        }
    }
}
