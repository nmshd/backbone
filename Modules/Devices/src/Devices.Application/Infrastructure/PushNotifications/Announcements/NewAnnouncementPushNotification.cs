using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Announcements;

public class NewAnnouncementPushNotification : IPushNotification
{
    public required string AnnouncementId { get; set; }
}
