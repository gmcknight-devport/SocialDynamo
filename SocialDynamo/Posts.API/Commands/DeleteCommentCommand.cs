using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Commands
{
    public class DeleteCommentCommand : IRequest<bool>
    {
        [Required]
        public Guid CommentId { get; set; } 
    }
}
