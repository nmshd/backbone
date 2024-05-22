using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.ExternalEvents;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.ExternalEventCreated;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.ExternalEventCreated;

public class ExternalEventCreatedDomainEventHandler : IDomainEventHandler<ExternalEventCreatedDomainEvent>
{
    private readonly IPushNotificationSender _pushSenderService;

    public ExternalEventCreatedDomainEventHandler(IPushNotificationSender pushSenderService)
    {
        _pushSenderService = pushSenderService;
    }

    public async Task Handle(ExternalEventCreatedDomainEvent @event)
    {
        await _pushSenderService.SendNotification(@event.Owner, new ExternalEventCreatedPushNotification(), CancellationToken.None);
    }
}
