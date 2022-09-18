using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Models
{
    public abstract record Like
    {
        public string LikeUserId { get; init; }
    }
}
