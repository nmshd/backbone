using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

public class IdentityDeletionProcessStartedIntegrationEvent : IntegrationEvent
{
    public IdentityDeletionProcessStartedIntegrationEvent(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
