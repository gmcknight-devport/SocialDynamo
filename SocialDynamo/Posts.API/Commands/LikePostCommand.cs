using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Commands
{
    public class LikePostCommand : IRequest<bool>
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string LikeUserId { get; set; }
    }
}
