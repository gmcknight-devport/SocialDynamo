using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Common.API.Profile.Commands
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
