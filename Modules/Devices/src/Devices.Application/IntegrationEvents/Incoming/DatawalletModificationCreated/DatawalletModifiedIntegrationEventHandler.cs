using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Datawallet;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated;

public class DatawalletModifiedIntegrationEventHandler : IIntegrationEventHandler<DatawalletModifiedIntegrationEvent>
{
    private readonly IPushService _pushService;

    public DatawalletModifiedIntegrationEventHandler(IPushService pushService)
    {
        _pushService = pushService;
    }

    public async Task Handle(DatawalletModifiedIntegrationEvent integrationEvent)
    {
        await _pushService.SendNotificationAsync(integrationEvent.Identity, new DatawalletModificationsCreatedPushNotification(integrationEvent.ModifiedByDevice));
    }
}
