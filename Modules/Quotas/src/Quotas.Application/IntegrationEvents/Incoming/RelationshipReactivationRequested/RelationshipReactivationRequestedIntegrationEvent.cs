using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipReactivationRequested;
public class RelationshipReactivationRequestedIntegrationEvent : IntegrationEvent
{
    public required string RelationshipId { get; set; } // todo: this may be unnecessary
    public required string RequestingIdentity { get; set; } // todo: check if there is a better name for this property
    public required string Peer { get; set; } // todo: this may be unnecessary
}
