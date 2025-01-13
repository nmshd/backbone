using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Announcements;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.AnnouncementCreated;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.AnnouncementCreated;

public class AnnouncementCreatedDomainEventHandler : IDomainEventHandler<AnnouncementCreatedDomainEvent>
{
    private readonly IPushNotificationSender _pushSenderService;

    public AnnouncementCreatedDomainEventHandler(IPushNotificationSender pushSenderService)
    {
        _pushSenderService = pushSenderService;
    }

    public async Task Handle(AnnouncementCreatedDomainEvent @event)
    {
        var pushNotificationTexts = @event.Texts.ToDictionary(k => k.Language, k => new NotificationText(k.Title, k.Body) { Title = k.Title, Body = k.Body });

        var recipientIdentityAddresses = @event.Recipients.Select(IdentityAddress.Parse).ToArray();
        await _pushSenderService.SendNotification(new NewAnnouncementPushNotification { AnnouncementId = @event.Id }, SendPushNotificationFilter.AllDevicesOf(recipientIdentityAddresses),
            pushNotificationTexts,
            CancellationToken.None);
    }
}
