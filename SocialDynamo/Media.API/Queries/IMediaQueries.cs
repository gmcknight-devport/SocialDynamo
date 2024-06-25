namespace Media.API.Queries
{
    public interface IMediaQueries
    {
        Task<Uri> GetBlob(string userId, string mediaItemId);
    }
}
