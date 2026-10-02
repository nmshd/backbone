using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.ExternalEvents;
using Backbone.Modules.Synchronization.Contracts.DomainEvents;

namespace Backbone.Modules.Devices.Module.Features.DomainEvents.ExternalEventCreated;

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

        if (identity is { Status: IdentityStatus.Active })
            await _pushSenderService.SendNotification(new ExternalEventCreatedPushNotification(), SendPushNotificationFilter.AllDevicesOf(@event.Owner), CancellationToken.None);
    }
}
