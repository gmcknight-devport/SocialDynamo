using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Media.API.Commands
{
    public class DeleteUserContainerCommand : IRequest<bool>
    {
        [Required]
        public string UserId { get; set; }
    }
}
