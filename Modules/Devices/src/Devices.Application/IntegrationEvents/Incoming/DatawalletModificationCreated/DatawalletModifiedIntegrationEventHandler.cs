using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Datawallet;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated;

public class DatawalletModifiedIntegrationEventHandler : IIntegrationEventHandler<DatawalletModifiedIntegrationEvent>
{
    private readonly IPushNotificationSender _pushSenderService;

    public DatawalletModifiedIntegrationEventHandler(IPushNotificationSender pushSenderService)
    {
        _pushSenderService = pushSenderService;
    }

    public async Task Handle(DatawalletModifiedIntegrationEvent integrationEvent)
    {
        await _pushSenderService.SendNotification(
            integrationEvent.Identity, 
            new DatawalletModificationsCreatedPushNotification(integrationEvent.ModifiedByDevice), 
            CancellationToken.None);
    }
}
