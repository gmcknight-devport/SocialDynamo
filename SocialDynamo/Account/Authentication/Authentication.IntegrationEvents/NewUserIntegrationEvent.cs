using Common;
using System.ComponentModel.DataAnnotations;

namespace Common.API.IntegrationEvents
{
    public record NewUserIntegrationEvent : IIntegrationEvent
    {
        [Required]
        public string UserId { get; set; }
    }
}
