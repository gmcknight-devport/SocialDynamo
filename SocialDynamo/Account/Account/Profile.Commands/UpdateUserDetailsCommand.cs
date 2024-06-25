using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Common.API.Profile.Commands
{
    public class UpdateUserDetailsCommand : IRequest<bool>
    {
        [Required]
        public string UserId { get; set; }
        [AllowNull]
        public string? Forename { get; set; }
        [AllowNull]
        public string? Surname { get; set; }
    }
}
