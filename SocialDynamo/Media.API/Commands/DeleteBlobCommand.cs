using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Media.API.Commands
{
    public class DeleteBlobCommand : IRequest<bool>
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string MediaItemId { get; set; }
    }
}
