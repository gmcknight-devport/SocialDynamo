using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Media.API.Commands
{
    public class UploadBlobCommand : IRequest<bool>
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string MediaItemId { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
