using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Device;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.BackupDeviceUsed;

public class BackupDeviceUsedDomainEventHandler : IDomainEventHandler<BackupDeviceUsedDomainEvent>
{
    private readonly IPushNotificationSender _pushNotificationSender;

    public BackupDeviceUsedDomainEventHandler(IPushNotificationSender pushNotificationSender)
    {
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(BackupDeviceUsedDomainEvent @event)
    {
        await _pushNotificationSender.SendNotification(new BackupDeviceUsedPushNotification(), SendPushNotificationFilter.AllDevicesOf(@event.IdentityAddress), CancellationToken.None);
    }
}
