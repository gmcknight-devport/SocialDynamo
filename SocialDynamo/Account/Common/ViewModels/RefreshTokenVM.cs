using System.ComponentModel.DataAnnotations;

namespace Account.API.Common.ViewModels
{
    public class RefreshTokenVM
    {
        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime TokenExpires { get; set; }  
    }
}
