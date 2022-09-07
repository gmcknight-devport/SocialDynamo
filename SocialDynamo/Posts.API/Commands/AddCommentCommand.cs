using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Commands
{
    public class AddCommentCommand : IRequest<bool>
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
