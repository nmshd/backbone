using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedIntegrationEvent : IntegrationEvent
{
    public required string Initiator { get; set; }
    public required string Peer { get; set; }
}
