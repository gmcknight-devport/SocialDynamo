
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

        public static MediaItemId Create(string authorId)
        {
            string id = GenerateId(authorId);

            return new MediaItemId(id);
        }

        private static string GenerateId(string authorId)
        {
            string mediaId = authorId + DateTime.UtcNow.ToString();
            mediaId = string.Join(string.Empty, mediaId.Where(c => !char.IsWhiteSpace(c)));
            return mediaId;
        }
    }
}
