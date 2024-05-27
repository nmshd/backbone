using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Xml;
using Backbone.BuildingBlocks.Domain.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Translations;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications;
public class PushNotificationTranslations
{
    [Fact]
    public void AllPushNotificationsHaveEnglishText()
    {
        var notificationStringsInResourceFile = GetStringsInResourceFile();
        var notificationTypes = GetNotificationTypes().ToArray();
        var titleNotifications = notificationTypes.Select(t => t.Name + ".Title");
        var bodyNotifications = notificationTypes.Select(t => t.Name + ".Body");
        var expectedNotificationStrings = titleNotifications.Concat(bodyNotifications);
        var missingStrings = expectedNotificationStrings.Except(notificationStringsInResourceFile);

        missingStrings.Should().BeEmpty();
    }

    private static IEnumerable<Type> GetNotificationTypes()
    {
        return typeof(TestPushNotification).Assembly.GetTypes().Where(t => typeof(IPushNotification).IsAssignableFrom(t) && !t.IsInterface);
    }

    private static IEnumerable<string> GetStringsInResourceFile()
    {
        var resourceManager = new ResourceManager("Backbone.Modules.Devices.Infrastructure.Properties.Resources", typeof(IPushNotificationResource).Assembly);
        resourceManager.ReleaseAllResources();

        var resourceSet = resourceManager.GetResourceSet(CultureInfo.GetCultureInfo(""), true, true);

        resourceSet.Should().NotBeNull();

        var xml = resourceSet!.GetString("PushNotifications.Translations.IPushNotificationResource.en");
        xml.Should().NotBeNull();

        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml!);
        var xmlNodeList = xmlDocument.DocumentElement!.SelectNodes("//data/@name");
        return xmlNodeList!.Cast<XmlNode>().ToList().Select(node => node.Value!).Where(v => v.EndsWith(".Title") || v.EndsWith(".Body"));
    }
}

