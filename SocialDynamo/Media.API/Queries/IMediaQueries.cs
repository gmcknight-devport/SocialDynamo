using Azure.Storage.Blobs.Models;

namespace Media.API.Queries
{
    public interface IMediaQueries
    {
        Task<BlobDownloadResult> GetBlob(string userId, string mediaItemId);
    }
}
