using Azure.Storage.Blobs.Models;

namespace Media.API.Queries
{
    public interface IMediaQueries
    {
        Task<BinaryData> GetBlob(string userId, string mediaItemId);
    }
}
