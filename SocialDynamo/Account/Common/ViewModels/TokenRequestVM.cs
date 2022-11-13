using System.ComponentModel.DataAnnotations;

namespace Account.API.ViewModels
{
    public record TokenRequestVM
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
