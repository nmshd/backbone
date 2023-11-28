using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedIntegrationEventHandler : IIntegrationEventHandler<IdentityDeletionProcessStartedIntegrationEvent>
{
    private readonly IPushNotificationSender _pushNotificationSender;

    public IdentityDeletionProcessStartedIntegrationEventHandler(IPushNotificationSender pushNotificationSender)
    {
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(IdentityDeletionProcessStartedIntegrationEvent @event)
    {
        var payload = new
        {
            Type = "IdentityDeletionProcessStarted",
            Message = "Deletion process started"
        };
        await _pushNotificationSender.SendNotification(@event.Address, payload, CancellationToken.None);
    }
}
