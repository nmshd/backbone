namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public interface IJwtGenerator
{
    Jwt Generate(string privateKey, string keyId, string teamId, string bundleId);
}
