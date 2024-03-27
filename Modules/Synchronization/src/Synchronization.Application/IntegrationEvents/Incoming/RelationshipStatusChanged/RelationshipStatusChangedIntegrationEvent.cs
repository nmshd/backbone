using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipStatusChanged;

public class RelationshipStatusChangedIntegrationEvent : IntegrationEvent
{
    public required string RelationshipId { get; set; }
    public required string Peer { get; set; }
}
