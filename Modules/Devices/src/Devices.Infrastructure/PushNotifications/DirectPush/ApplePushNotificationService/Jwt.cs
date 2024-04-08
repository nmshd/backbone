using Backbone.Tooling;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class Jwt
{
    public readonly string Value;
    private readonly DateTime _createdAt;

    public Jwt(string value)
    {
        Value = value;
        _createdAt = SystemTime.UtcNow;
    }

    public bool IsExpired()
    {
        return _createdAt.AddMinutes(50) < SystemTime.UtcNow;
    }
}
