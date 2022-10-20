using Account.Tests.Exceptions;
using Azure.Storage.Blobs;
using MediatR;

namespace Media.API.Commands
{
    public class AddUserBlobContainerCommandHandler : IRequestHandler<AddUserBlobContainerCommand, bool>
    {
        private readonly IConfiguration _configuration;

        public AddUserBlobContainerCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Handle(AddUserBlobContainerCommand command, CancellationToken cancellationToken)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["ConnectionStrings:AzureStorage"]);
            var container = blobServiceClient.GetBlobContainerClient(command.UserId);

            if (container.Exists()) 
                throw new DuplicateUserContainerException("User container already exists, cannot create duplicate");

            await container.CreateAsync();
            return true;
        }
    }
}
