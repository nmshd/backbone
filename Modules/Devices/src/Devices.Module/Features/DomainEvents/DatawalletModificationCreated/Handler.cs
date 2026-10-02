using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.Datawallet;
using Backbone.Modules.Synchronization.Contracts.DomainEvents;

namespace Backbone.Modules.Devices.Module.Features.DomainEvents.DatawalletModificationCreated;

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
