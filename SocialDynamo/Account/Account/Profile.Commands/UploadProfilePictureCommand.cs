using Common.Extensions;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Account.API.Profile.Commands
{
    public record UploadProfilePictureCommand : IRequest<bool>
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [AllowedExtensions(new string[] { ".jpg",".png",})]
        public IFormFile ProfilePicture { get; set; }
    }
}
