using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;

public interface IPushNotificationTextProvider
{
    Dictionary<CommunicationLanguage, NotificationText> GetNotificationTextsForLanguages(Type pushNotificationType, IEnumerable<CommunicationLanguage> languages);
}
