using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
public class TierOfIdentityChangedIntegrationEvent : IntegrationEvent
{
    public required string OldTier { get; set; }
    public required string NewTier { get; set; }
    public required string IdentityAddress { get; set; }
}
