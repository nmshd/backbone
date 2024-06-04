namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;

public interface IJwtGenerator
{
    Jwt Generate(string privateKey, string keyId, string teamId, string bundleId);
}
