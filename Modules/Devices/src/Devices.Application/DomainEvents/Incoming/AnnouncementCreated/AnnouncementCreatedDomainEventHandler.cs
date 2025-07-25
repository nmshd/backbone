﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Announcements;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.AnnouncementCreated;
using Backbone.Tooling.Extensions;

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
        await SendPushNotificationForAnnouncement(@event);
    }

    private async Task SendPushNotificationForAnnouncement(AnnouncementCreatedDomainEvent @event)
    {
        if (@event.IsSilent)
            return;

        var pushNotificationTexts = @event.Texts.ToDictionary(k => k.Language, k => new NotificationText(k.Title, k.Body) { Title = k.Title, Body = k.Body });

        var pushNotificationFilter = @event.Recipients.IsEmpty()
            ? SendPushNotificationFilter.AllDevicesOfAllIdentities()
            : SendPushNotificationFilter.AllDevicesOf(@event.Recipients.Select(IdentityAddress.Parse).ToArray());

        await _pushSenderService.SendNotification(new NewAnnouncementPushNotification { AnnouncementId = @event.Id }, pushNotificationTexts, pushNotificationFilter, CancellationToken.None);
    }
}
