using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.Announcements;

public class NewAnnouncementPushNotification : IPushNotification
{
    public required string AnnouncementId { get; set; }
}
