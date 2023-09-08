using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
public class TierOfIdentityChangedIntegrationEvent : IntegrationEvent
{
    public string OldTier { get; set; }
    public string NewTier { get; set; }
    public string IdentityAddress { get; set; }
}
