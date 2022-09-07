using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Commands
{
    public class DeletePostCommand : IRequest<bool>
    {
        [Required]
        public Guid PostId { get; set; }        
    }
}
