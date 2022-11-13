using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Media.API.Exceptions;

namespace Media.API.Queries
{
    public class MediaQueries : IMediaQueries
    {
        private readonly IConfiguration _configuration;

        public MediaQueries(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Returns a blob for the specified UserId using the MediaItemId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        /// <exception cref="NoUserContainerException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<BinaryData> GetBlob(string userId, string mediaItemId)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration["AzureStorage:ConnectionString"]);
            var container = blobServiceClient.GetBlobContainerClient(userId);

            if (!container.Exists())
                throw new NoUserContainerException("No user container found");

            BlobDownloadResult blob = await container.GetBlobClient(mediaItemId).DownloadContentAsync();
            var result = blob.Content;
            
            if(result == null)
                throw new NullReferenceException(nameof(blob));

            return result;
        }
    }
}
