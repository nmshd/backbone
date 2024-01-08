using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.PeerIdentityToBeDeleted;

public class PeerIdentityToBeDeletedIntegrationEvent : IntegrationEvent
{
    public string Address { get; set; }
    public string DeletionProcessId { get; set; }
    public string RelationshipId { get; set; }
}
