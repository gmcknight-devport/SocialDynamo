using Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".mp4", ".mov" })]        
        public IFormFile File { get; set; }
    }
}
