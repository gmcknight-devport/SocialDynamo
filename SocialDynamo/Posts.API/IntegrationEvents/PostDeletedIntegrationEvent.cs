﻿using Common;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.IntegrationEvents
{
    public record PostDeletedIntegrationEvent : IIntegrationEvent
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public ICollection<string> MediaItemIds { get; set; }
    }
}
