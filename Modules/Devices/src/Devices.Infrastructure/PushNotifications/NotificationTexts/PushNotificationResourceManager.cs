﻿using System.Resources;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;

public class PushNotificationResourceManager : ResourceManager
{
    public PushNotificationResourceManager() : base("Backbone.Modules.Devices.Infrastructure.Translations.Resources", typeof(PushNotificationResourceManager).Assembly)
    {
    }
}
