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
        public Guid Id { get; set; }
        internal MediaItemId(Guid value)
        {
            Id = value;
        }

        public static MediaItemId Create(int authorId)
        {
            Guid id = GenerateId(authorId);

            return new MediaItemId(id);
        }

        private static Guid GenerateId(int authorId)
        {
            Random random = new Random();

            byte[] bytes = BitConverter.GetBytes(random.Next());
            short secondPart = BitConverter.ToInt16(bytes, 0);
            short thirdPart = BitConverter.ToInt16(bytes, 2);

            byte[] fourthPart = new byte[8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(fourthPart);

            Guid id = new Guid(authorId, secondPart, thirdPart, fourthPart);

            return id;
        }
    }
}
