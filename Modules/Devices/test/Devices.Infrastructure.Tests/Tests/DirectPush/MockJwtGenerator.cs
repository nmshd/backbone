using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;

public class MockJwtGenerator : IJwtGenerator
{
    public static int NumberOfGeneratedTokens = 0;

    public Jwt Generate(string privateKey, string keyId, string teamId)
    {
        NumberOfGeneratedTokens++;
        return new Jwt("some-valid-jwt");
    }
}
