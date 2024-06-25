using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain.ViewModels
{
    public class RefreshTokenVM
    {
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public DateTime RefreshExpires { get; set; }
    }
}
