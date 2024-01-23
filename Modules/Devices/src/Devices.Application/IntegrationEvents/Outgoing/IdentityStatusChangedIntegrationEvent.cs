using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
public class IdentityStatusChangedIntegrationEvent : IntegrationEvent
{
    public IdentityStatusChangedIntegrationEvent(IdentityAddress identityAddress, IdentityStatus newStatus) : base($"{identityAddress.StringValue}/StatusChanged")
    {
        IdentityAddress = identityAddress;
        NewStatus = newStatus;
    }

    public IdentityAddress IdentityAddress { get; set; }
    public IdentityStatus NewStatus { get; set; }
}
