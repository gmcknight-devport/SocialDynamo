using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Common.OptionsConfig;
using Media.API.Exceptions;
using Microsoft.Extensions.Options;

namespace Media.API.Queries
{
    public class MediaQueries : IMediaQueries
    {
        private readonly string _azureStorage;
        private readonly ILogger<MediaQueries> _logger;

        public MediaQueries(IConfiguration configuration, IOptions<ConnectionOptions> options, ILogger<MediaQueries> logger)
        {

            if (configuration["AzureStorage"] != null)
                _azureStorage = configuration["AzureStorage"];
            else
                _azureStorage = options.Value.AzureStorage;

            _logger = logger;
        }

        /// <summary>
        /// Returns a blob for the specified UserId using the MediaItemId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        /// <exception cref="NoUserContainerException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<Uri> GetBlob(string userId, string mediaItemId)
        {
            try
            {
                BlobServiceClient client = new BlobServiceClient(_azureStorage);
                var container = client.GetBlobContainerClient(userId.ToLower());

                if (!container.Exists())
                    throw new NoUserContainerException("No user container found");

                string newId = mediaItemId.Replace("/", "%2F").Replace(":", "%3A");
                BlobClient blob = container.GetBlobClient(newId);

                return await BlobSASToken(blob);
            }
            catch (Exception e)
            {
                throw new Exception("Unexpected error occurred");
            }        
        }

        private async Task<Uri> BlobSASToken(BlobClient blobClient)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b"
            };
            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(1);
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            _logger.LogInformation("----- SAS URI has been generated");

            return blobClient.GenerateSasUri(sasBuilder);
        }
    }
}
