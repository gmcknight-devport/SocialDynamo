using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.ViewModels
{
    public record LikeVM
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string LikeUserId { get; set; }
    }
}
