using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Media.API.Commands
{
    public class AddUserBlobContainerCommand : IRequest<bool>
    {
        [Required]
        public string UserId { get; set; }
    }
}
