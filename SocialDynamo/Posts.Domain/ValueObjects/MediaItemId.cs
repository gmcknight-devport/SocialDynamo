using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.ValueObjects
{
    public record MediaItemId
    {
        public string Id { get; set; }
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
            return mediaId;
        }
    }
}
