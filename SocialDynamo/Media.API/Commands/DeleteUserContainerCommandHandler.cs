using Azure.Storage.Blobs;
using Common.OptionsConfig;
using Media.API.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Media.API.Commands
{
    //Command handler to delete a user container. 
    public class DeleteUserContainerCommandHandler : IRequestHandler<DeleteUserContainerCommand, bool>
    {
        private readonly ILogger<DeleteUserContainerCommandHandler> _logger;
        private readonly BlobServiceClient _client;

        public DeleteUserContainerCommandHandler(IConfiguration baseConfiguration,
                                                 IOptions<ConnectionOptions> optionsConfiguration,
                                                 ILogger<DeleteUserContainerCommandHandler> logger)
        {
            _logger = logger;

            //Validation workaround to allow connectionstring to be found in dev or prod.
            if (baseConfiguration["ServiceBus"] != null)
                _client = new BlobServiceClient(baseConfiguration["AzureStorage"]);
            else
                _client = new BlobServiceClient(optionsConfiguration.Value.AzureStorage);
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
            var container = _client.GetBlobContainerClient(command.UserId.ToLower());

            if (!container.Exists())
                throw new NoUserContainerException("No user container to delete");

            _logger.LogInformation("----- Container of user deleted, User: {@UserId}", command.UserId);

            await container.DeleteAsync();

            return true;
        }
    }
}
