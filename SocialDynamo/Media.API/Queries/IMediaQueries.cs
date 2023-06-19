namespace Media.API.Queries
{
    public interface IMediaQueries
    {
        Task<byte[]> GetBlob(string userId, string mediaItemId);
    }
}
