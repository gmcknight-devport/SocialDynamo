using Azure.Storage.Blobs;
using Media.API.Exceptions;
using MediatR;

namespace Media.API.Commands
{
    public class DeleteBlobCommandHandler : IRequestHandler<DeleteBlobCommand, bool>
    {
        private readonly IConfiguration _configuration;

        public DeleteBlobCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Handle(DeleteBlobCommand command, CancellationToken cancellationToken)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["ConnectionString"]);
            var container = blobServiceClient.GetBlobContainerClient(command.UserId);
            

            if (!container.Exists())
                throw new NoUserContainerException("No user container to find blob to delete");

            await container.DeleteBlobIfExistsAsync(command.MediaItemId);

            return true;
        }
    }
}
