using Common;
using System.ComponentModel.DataAnnotations;

namespace Account.API.Profile.IntegrationEvents
{
    public record DeleteUserIntegrationEvent : IIntegrationEvent
    {
        [Required]
        public string UserId { get; set; }
    }
}
