using System.Reflection;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

[NotificationText(Title = "Your Identity will be deleted")]
public record DeletionStartsNotification
{
    public DeletionStartsNotification(string message) => GetType().GetCustomAttribute<NotificationTextAttribute>().Body = message;
};
