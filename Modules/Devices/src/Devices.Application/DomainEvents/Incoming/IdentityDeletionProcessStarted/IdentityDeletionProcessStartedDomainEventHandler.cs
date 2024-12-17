using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;

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
