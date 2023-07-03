using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.ExternalEvents;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;

public class ExternalEventCreatedIntegrationEventHandler : IIntegrationEventHandler<ExternalEventCreatedIntegrationEvent>
{
    private readonly IPushService _pushService;

    public ExternalEventCreatedIntegrationEventHandler(IPushService pushService)
    {
        _pushService = pushService;
    }

    public async Task Handle(ExternalEventCreatedIntegrationEvent @event)
    {
        await _pushService.SendNotification(@event.Owner, new ExternalEventCreatedPushNotification());
    }
}
