namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class ApnsJwtCache
{
    private Jwt _jwt;
    public bool HasValue() => _jwt != null && !_jwt.IsExpired();
    public Jwt GetValue() => _jwt;
    public void UpdateValue(Jwt jwt) => _jwt = jwt;
}
