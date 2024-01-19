namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public class NotificationIdAttribute : Attribute
{
    public NotificationIdAttribute(int value)
    {
        Value = value;
    }

    public int? Value { get; }
}
