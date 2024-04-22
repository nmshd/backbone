using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipReactivationRequested;
public class RelationshipReactivationRequestedIntegrationEvent : IntegrationEvent
{
    public required string RelationshipId { get; set; }
    public required string RequestingIdentity { get; set; }
    public required string Peer { get; set; }
}
