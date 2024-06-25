using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Commands
{
    public class AddCommentCommand : IRequest<Guid>
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
