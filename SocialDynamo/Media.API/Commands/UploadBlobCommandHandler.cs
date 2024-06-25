using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Common.OptionsConfig;
using Media.API.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Media.API.Commands
{
    //Command handler to upload a blob.
    public class UploadBlobCommandHandler : IRequestHandler<UploadBlobCommand, bool>
    {
        private readonly ILogger<UploadBlobCommandHandler> _logger;
        private readonly BlobServiceClient _client;

        public UploadBlobCommandHandler(IConfiguration baseConfiguration,
                                        IOptions<ConnectionOptions> optionsConfiguration,
                                        ILogger<UploadBlobCommandHandler> logger)
        {
            _logger = logger;

            //Validation workaround to allow connectionstring to be found in dev or prod.
            if (baseConfiguration["AzureStorage"] != null)
                _client = new BlobServiceClient(baseConfiguration["AzureStorage"]);
            else
                _client = new BlobServiceClient(optionsConfiguration.Value.AzureStorage);
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
            var container = _client.GetBlobContainerClient(command.UserId.ToLower());

            if (!container.Exists())
                throw new NoUserContainerException("No user container to upload blob");

            //Upload blob
            var finalId = command.MediaItemId.Replace("/", "%2F").Replace(":", "%3A");
            BlobClient blob = container.GetBlobClient(finalId);

            await using (Stream stream = command.File.OpenReadStream())
            {                
                //Upload
                await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = "image/jpeg" });
            }

            _logger.LogInformation("----- Blob uploaded for specified user. User: {@UserId}", command.UserId);

            return true;
        }
    }
}
