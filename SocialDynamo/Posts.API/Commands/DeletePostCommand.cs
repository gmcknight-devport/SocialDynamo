using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Commands
{
    public class DeletePostCommand : IRequest<bool>
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public Guid PostId { get; set; }        
    }
}
