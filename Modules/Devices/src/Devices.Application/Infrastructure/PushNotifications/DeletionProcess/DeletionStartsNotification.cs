﻿using System.Reflection;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

[NotificationText(Title = "Your Identity will be deleted", Body = "")]
public record DeletionStartsNotification
{
    public DeletionStartsNotification() => GetType().GetCustomAttribute<NotificationTextAttribute>()!.Body = IdentityDeletionConfiguration.DeletionStartsNotification.Text;
};
