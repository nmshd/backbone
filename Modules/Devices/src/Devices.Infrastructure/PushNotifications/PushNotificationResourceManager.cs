using System.Resources;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public class PushNotificationResourceManager : ResourceManager
{
    public PushNotificationResourceManager() : base("Backbone.Modules.Devices.Infrastructure.Translations.Resources", typeof(PushNotificationTextProvider).Assembly)
    {
    }
}
