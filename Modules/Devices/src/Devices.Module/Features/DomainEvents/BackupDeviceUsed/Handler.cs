using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.Device;

namespace Backbone.Modules.Devices.Module.Features.DomainEvents.BackupDeviceUsed;

public class Handler : IDomainEventHandler<BackupDeviceUsedDomainEvent>
{
    private readonly IPushNotificationSender _pushNotificationSender;

    public Handler(IPushNotificationSender pushNotificationSender)
    {
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(BackupDeviceUsedDomainEvent @event)
    {
        await _pushNotificationSender.SendNotification(new BackupDeviceUsedPushNotification(), SendPushNotificationFilter.AllDevicesOf(@event.IdentityAddress), CancellationToken.None);
    }
}
