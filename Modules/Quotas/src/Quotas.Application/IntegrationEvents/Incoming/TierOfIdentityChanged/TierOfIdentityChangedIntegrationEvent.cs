using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
public class TierOfIdentityChangedIntegrationEvent : IntegrationEvent
{
    public string OldTierId { get; set; }
    public string NewTierId { get; set; }
    public string IdentityAddress { get; set; }
}
