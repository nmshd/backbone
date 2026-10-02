using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.DeletionProcess;

namespace Backbone.Modules.Devices.Module.Features.DomainEvents.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedDomainEventHandler : IDomainEventHandler<IdentityDeletionProcessStartedDomainEvent>
{
    private readonly IPushNotificationSender _pushNotificationSender;

    public IdentityDeletionProcessStartedDomainEventHandler(IPushNotificationSender pushNotificationSender)
    {
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(IdentityDeletionProcessStartedDomainEvent @event)
    {
        await _pushNotificationSender.SendNotification(new DeletionProcessStartedPushNotification(), SendPushNotificationFilter.AllDevicesOf(@event.Address), CancellationToken.None);
    }
}
