﻿using MediatR;
using Posts.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Commands
{
    public class UpdatePostCommand : IRequest<bool>
    {
        [Required]
        public Guid PostId { get; set; }

        public string? Hashtag { get; set; }

        [MaxLength(2200)]
        public string? Caption { get; set; }
    }
}
