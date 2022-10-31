using Common;
using System.ComponentModel.DataAnnotations;

namespace Account.API.Account.Profile.IntegrationEvents
{
    public record NewUserIntegrationEvent : IIntegrationEvent
    {
        [Required]
        public string UserId { get; set; }
    }
}
