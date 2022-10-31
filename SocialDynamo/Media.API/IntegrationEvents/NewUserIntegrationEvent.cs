using Common;
using System.ComponentModel.DataAnnotations;

namespace Media.API.IntegrationEvents
{
    public record NewUserIntegrationEvent : IIntegrationEvent
    {
        [Required]
        public string UserId { get; set; }
    }
}
