using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
public class PeerIdentityDeletedIntegrationEvent : IntegrationEvent
{
    public PeerIdentityDeletedIntegrationEvent(string relationshipId, IdentityAddress identityAddress) : base($"{relationshipId}/PeerIdentityDeleted/${identityAddress}")
    {
        IdentityAddress = identityAddress;
        RelationshipId = relationshipId;
    }

    public string IdentityAddress { get; }
    public string RelationshipId { get; }
}
