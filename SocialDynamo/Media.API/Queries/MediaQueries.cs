using Azure.Storage.Blobs;
using Common.OptionsConfig;
using Media.API.Exceptions;
using Microsoft.Extensions.Options;

namespace Media.API.Queries
{
    public class MediaQueries : IMediaQueries
    {
        private readonly string _azureStorage;

        public MediaQueries(IConfiguration configuration, IOptions<ConnectionOptions> options)
        {

            if (configuration["AzureStorage"] != null)
                _azureStorage = configuration["AzureStorage"];
            else
                _azureStorage = options.Value.AzureStorage;
        }

        /// <summary>
        /// Returns a blob for the specified UserId using the MediaItemId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        /// <exception cref="NoUserContainerException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<byte[]> GetBlob(string userId, string mediaItemId)
        {
            try
            {
                BlobContainerClient container = new BlobContainerClient(_azureStorage, userId);                
                string newId = mediaItemId.Replace("%2F", "/");
                newId = newId.Replace("%3a", ":");
                
                if (!container.Exists())
                    throw new NoUserContainerException("No user container found");

                using(MemoryStream memoryStream = new())
                {
                    await container.GetBlobClient(newId).DownloadToAsync(memoryStream);
                    memoryStream.Position = 0;

                    return memoryStream.ToArray();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.StackTrace);
            }        
        }
    }
}
