using Media.API.Exceptions;
using Azure.Storage.Blobs;
using MediatR;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Common.OptionsConfig;
using System.ComponentModel;
using Azure.Storage.Blobs.Models;

namespace Media.API.Commands
{
    //Handles command - adds a user container azure storage. 
    public class AddUserBlobContainerCommandHandler : IRequestHandler<AddUserBlobContainerCommand, bool>
    {
        private readonly ILogger<AddUserBlobContainerCommand> _logger;
        private readonly string _connectionString;

        public AddUserBlobContainerCommandHandler(IConfiguration baseConfiguration, 
                                              IOptions<ConnectionOptions> optionsConfiguration, 
                                              ILogger<AddUserBlobContainerCommand> logger)
        {
            _logger = logger;

            //Validation workaround to allow connectionstring to be found in dev or prod.
            if (baseConfiguration["AzureStorage"] != null)
                _connectionString = baseConfiguration["AzureStorage"];
            else
                _connectionString = optionsConfiguration.Value.AzureStorage;
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
            BlobContainerClient _client = new BlobContainerClient(_connectionString, command.UserId.ToLower());
            await _client.CreateIfNotExistsAsync(PublicAccessType.None, null, CancellationToken.None);

            _logger.LogInformation("----- User container added, User: {@UserId}", command.UserId);

            return true;
        }
    }
}
