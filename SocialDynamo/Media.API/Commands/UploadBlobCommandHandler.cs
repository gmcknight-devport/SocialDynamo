using Azure.Storage.Blobs;
using Media.API.Exceptions;
using MediatR;

namespace Media.API.Commands
{
    public class UploadBlobCommandHandler : IRequestHandler<UploadBlobCommand, bool>
    {
        private readonly IConfiguration _configuration;

        public UploadBlobCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Handle(UploadBlobCommand command, CancellationToken cancellationToken)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["ConnectionString"]);
            var container = blobServiceClient.GetBlobContainerClient(command.UserId);

            if (!container.Exists())
                throw new NoUserContainerException("No user container to upload blob");

            //Upload blob
            BlobClient blob = container.GetBlobClient(command.MediaItemId);
            await using (Stream stream = command.File.OpenReadStream())
            {
                //Scan for viruses
                FileScanner.FileScanner.Run(stream);

                //Upload
                await blob.UploadAsync(stream);
            }
            return true;
        }
    }
}
