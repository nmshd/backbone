using System.Resources;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Xunit;
using EnglishTranslations = Backbone.Modules.Devices.Infrastructure.Translations.resources;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications;
public class PushNotificationTranslations
{
    [Fact]
    public void A()
    {
        var resourceManager  = new ResourceManager("Backbone.Modules.Devices.Infrastructure.Translations", typeof(EnglishTranslations).Assembly);
        //EnglishTranslations.DeletionProcessApprovedNotification_Message;

        var s = resourceManager.GetString("DeletionProcessApprovedNotification.Body");
    }
}
