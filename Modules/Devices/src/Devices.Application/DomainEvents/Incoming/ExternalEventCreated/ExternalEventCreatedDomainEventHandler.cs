using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.ExternalEvents;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.ExternalEventCreated;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.ExternalEventCreated;

public class ExternalEventCreatedDomainEventHandler : IDomainEventHandler<ExternalEventCreatedDomainEvent>
{
    private readonly IPushNotificationSender _pushSenderService;
    private readonly IIdentitiesRepository _identitiesRepository;

    public ExternalEventCreatedDomainEventHandler(IPushNotificationSender pushSenderService, IIdentitiesRepository identitiesRepository)
    {
        _pushSenderService = pushSenderService;
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(ExternalEventCreatedDomainEvent @event)
    {
        if (@event.IsDeliveryBlocked)
            return;

        var identity = await _identitiesRepository.Get(@event.Owner, CancellationToken.None);

        if (identity is { IsToBeDeleted: false })
            await _pushSenderService.SendNotification(new ExternalEventCreatedPushNotification(), SendPushNotificationFilter.AllDevicesOf(@event.Owner), CancellationToken.None);
    }
}
