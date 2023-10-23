using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Devices.Application.Infrastructure.PushNotifications.ExternalEvents;

namespace Backbone.Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;

public class ExternalEventCreatedIntegrationEventHandler : IIntegrationEventHandler<ExternalEventCreatedIntegrationEvent>
{
    private readonly IPushService _pushService;

    public ExternalEventCreatedIntegrationEventHandler(IPushService pushService)
    {
        _pushService = pushService;
    }

    public async Task Handle(ExternalEventCreatedIntegrationEvent @event)
    {
        await _pushService.SendNotification(@event.Owner, new ExternalEventCreatedPushNotification(), CancellationToken.None);
    }
}
