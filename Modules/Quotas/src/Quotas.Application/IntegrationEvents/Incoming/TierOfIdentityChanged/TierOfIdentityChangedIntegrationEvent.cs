using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
public class TierOfIdentityChangedIntegrationEvent : IntegrationEvent
{
    public required string OldTierId { get; set; }
    public required string NewTierId { get; set; }
    public required string IdentityAddress { get; set; }
}
