using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.ExternalEvents;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;

public class ExternalEventCreatedIntegrationEventHandler : IIntegrationEventHandler<ExternalEventCreatedIntegrationEvent>
{
    private readonly IPushNotificationSender _pushSenderService;

    public ExternalEventCreatedIntegrationEventHandler(IPushNotificationSender pushSenderService)
    {
        _pushSenderService = pushSenderService;
    }

    public async Task Handle(ExternalEventCreatedIntegrationEvent @event)
    {
        await _pushSenderService.SendNotification(@event.Owner, new ExternalEventCreatedPushNotification(), CancellationToken.None);
    }
}
