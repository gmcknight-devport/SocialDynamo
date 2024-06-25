using System.ComponentModel.DataAnnotations;

namespace Common.API.Commands
{
    public class RefreshJwtTokenCommand
    {
        [Required]
        public string UserId { get; set; }
    }
}
