﻿namespace Backbone.Devices.Application.Infrastructure.PushNotifications.Datawallet;

[NotificationText(Title = NotificationTextAttribute.DEFAULT_TITLE, Body = NotificationTextAttribute.DEFAULT_BODY)]
public record DatawalletModificationsCreatedPushNotification(string CreatedByDevice);
