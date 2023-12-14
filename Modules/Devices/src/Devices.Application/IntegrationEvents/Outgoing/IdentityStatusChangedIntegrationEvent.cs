using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
public class IdentityStatusChangedIntegrationEvent(IdentityAddress identityAddress, IdentityStatus newStatus) : IntegrationEvent($"{identityAddress.StringValue}/StatusChanged")
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
    public IdentityStatus Status { get; set; } = newStatus;
}
