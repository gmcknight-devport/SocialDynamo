
namespace SocialDynamoAPI.BaseAggregator.ValueObjects
{
    public record MediaItemId
    {
        public string Id { get; set; }

        private MediaItemId() { }
        internal MediaItemId(string value)
        {
            Id = value;
        }

        public static MediaItemId Create(string authorId, int mediaItemNum)
        {
            string id = GenerateId(authorId, mediaItemNum);

            return new MediaItemId(id);
        }

        private static string GenerateId(string authorId, int mediaItemNum)
        {
            string mediaId = authorId + DateTime.UtcNow.ToString().Replace("/", "%2F").Replace(":", "%3A") + "-" + mediaItemNum;
            mediaId = string.Join(string.Empty, mediaId.Where(c => !char.IsWhiteSpace(c)));
            return mediaId;
        }
    }
}
