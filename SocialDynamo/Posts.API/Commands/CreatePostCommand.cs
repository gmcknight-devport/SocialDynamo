using MediatR;
using Posts.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Commands
{
    public class CreatePostCommand : IRequest<bool>
    {
        [Required]
        public string AuthorId{ get; set; }

        public string Hashtag { get; set; }

        [Required]
        public string Caption { get; set; }

        [Required]
        public List<MediaItemId> MediaItemIds { get; set; }
    }
}
