using Azure.Storage.Blobs;
using Media.API.Exceptions;
using MediatR;

namespace Media.API.Commands
{
    public class DeleteUserContainerCommandHandler : IRequestHandler<DeleteUserContainerCommand, bool>
    {
        private readonly IConfiguration _configuration;

        public DeleteUserContainerCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Handle(DeleteUserContainerCommand command, CancellationToken cancellationToken)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["ConnectionStrings:AzureStorage"]);
            var container = blobServiceClient.GetBlobContainerClient(command.UserId);

            if (!container.Exists())
                throw new NoUserContainerException("No user container to delete");

            await container.DeleteAsync();

            return true;
        }
    }
}
