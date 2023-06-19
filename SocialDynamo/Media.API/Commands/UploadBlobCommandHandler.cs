using Azure.Storage.Blobs;
using Media.API.Exceptions;
using MediatR;

namespace Media.API.Commands
{
    //Command handler to upload a blob.
    public class UploadBlobCommandHandler : IRequestHandler<UploadBlobCommand, bool>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UploadBlobCommandHandler> _logger;

        public UploadBlobCommandHandler(IConfiguration configuration, ILogger<UploadBlobCommandHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Handle method of mediatr interface - uploads a blob for the specified 
        /// user once container has been validated.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NoUserContainerException"></exception>
        public async Task<bool> Handle(UploadBlobCommand command, CancellationToken cancellationToken)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["AzureStorage"]);
            var container = blobServiceClient.GetBlobContainerClient(command.UserId);

            if (!container.Exists())
                throw new NoUserContainerException("No user container to upload blob");

            //Upload blob
            BlobClient blob = container.GetBlobClient(command.MediaItemId);
            await using (Stream stream = command.File.OpenReadStream())
            {                
                //Upload
                await blob.UploadAsync(stream);
            }

            _logger.LogInformation("----- Blob uploaded for specified user. User: {@UserId}", command.UserId);

            return true;
        }
    }
}
