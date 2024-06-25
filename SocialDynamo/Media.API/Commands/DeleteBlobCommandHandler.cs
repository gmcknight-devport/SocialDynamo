using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Common.OptionsConfig;
using Media.API.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Media.API.Commands
{
    //Handles command - deleted a specified blob from the user container. 
    public class DeleteBlobCommandHandler : IRequestHandler<DeleteBlobCommand, bool>
    {
        private readonly ILogger<DeleteBlobCommandHandler> _logger;
        private readonly BlobServiceClient _client;
        private readonly BlobContainerClient _containerClient;

        public DeleteBlobCommandHandler(IConfiguration baseConfiguration,
                                        IOptions<ConnectionOptions> optionsConfiguration, 
                                        ILogger<DeleteBlobCommandHandler> logger)
        {
            _logger = logger;

            //Validation workaround to allow connectionstring to be found in dev or prod.
            if (baseConfiguration["AzureStorage"] != null)
                _client = new BlobServiceClient(baseConfiguration["AzureStorage"]);
            else
                _client = new BlobServiceClient(optionsConfiguration.Value.AzureStorage);
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
            var container = _client.GetBlobContainerClient(command.UserId.ToLower());
            
            if (!container.Exists())
                throw new NoUserContainerException("No user container to find blob to delete");

            _logger.LogInformation("----- Container has been found, deleting blob if it exists");

            await container.DeleteBlobIfExistsAsync(command.MediaItemId);

            return true;
        }
    }
}
