using System.ComponentModel.DataAnnotations;

namespace Common.API.ViewModels
{
    public record TokenRequestVM
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
