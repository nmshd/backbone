namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public class NotificationIdAttribute : Attribute
{
    public NotificationIdAttribute(string value)
    {
        if (value.Length > 64)
            throw new Exception("The value of the NotificationIdAttribute must be less than or equal to 64 characters.");

        Value = value;
    }

    public string Value { get; }
}
