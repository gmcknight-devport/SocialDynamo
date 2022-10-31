using Common;
using System.ComponentModel.DataAnnotations;

namespace Account.API.Account.Profile.IntegrationEvents
{
    public record PostDeletedIntegrationEvent : IIntegrationEvent
    {
        [Required]
        public ICollection<string> MediaItemIds { get; set; }
    }
}
