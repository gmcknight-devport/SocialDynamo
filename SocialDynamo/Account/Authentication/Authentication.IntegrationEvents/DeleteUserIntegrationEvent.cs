using Common;
using System.ComponentModel.DataAnnotations;

namespace Account.API.Account.Authentication.IntegrationEvents
{
    public record DeleteUserIntegrationEvent : IIntegrationEvent
    {
        [Required]
        public string UserId { get; set; }
    }
}
