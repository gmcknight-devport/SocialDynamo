using Posts.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.ViewModels
{
    public record IdVM
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public MediaItemId MediaItemIds { get; set; }
    }
}
