using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Datawallet;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.DatawalletModificationCreated;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.DatawalletModificationCreated;

public class DatawalletModifiedDomainEventHandler : IDomainEventHandler<DatawalletModifiedDomainEvent>
{
    private readonly IPushNotificationSender _pushSenderService;

    public DatawalletModifiedDomainEventHandler(IPushNotificationSender pushSenderService)
    {
        _pushSenderService = pushSenderService;
    }

    public async Task Handle(DatawalletModifiedDomainEvent domainEvent)
    {
        var notification = new DatawalletModificationsCreatedPushNotification(domainEvent.ModifiedByDevice);
        await _pushSenderService.SendNotification(
            notification,
            SendPushNotificationFilter.AllDevicesOfExcept(IdentityAddress.ParseUnsafe(domainEvent.Identity), DeviceId.Parse(domainEvent.ModifiedByDevice)),
            CancellationToken.None);
    }
}
