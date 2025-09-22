using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Tests.PushNotifications;

public class PnsRegistrationTests : AbstractTestsBase
{
    [Fact]
    public void Generate_DevicePushIdentifier_while_instancing_PnsRegistration()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "someValue").Value;

        SystemTime.Set(SystemTime.UtcNow);

        // Act
        var pnsRegistration = new PnsRegistration(identityAddress, deviceId, pnsHandle, "someAppId", PushEnvironment.Development);

        // Assert
        pnsRegistration.IdentityAddress.ShouldBe(identityAddress);
        pnsRegistration.DeviceId.ShouldNotBeNull();
        pnsRegistration.DevicePushIdentifier.ShouldNotBeNull();
        pnsRegistration.Handle.ShouldBe(pnsHandle);
        pnsRegistration.UpdatedAt.ShouldBe(SystemTime.UtcNow);
        pnsRegistration.AppId.ShouldBe("someAppId");
        pnsRegistration.Environment.ShouldBe(PushEnvironment.Development);
    }

    [Fact]
    public void Generate_new_DevicePushIdentifier_on_every_PnsRegistration_instance()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "someValue").Value;

        var otherPnsRegistration = new PnsRegistration(identityAddress, deviceId, pnsHandle, "someAppId", PushEnvironment.Development);

        // Act
        var pnsRegistration = new PnsRegistration(identityAddress, deviceId, pnsHandle, "someAppId", PushEnvironment.Development);

        // Assert
        pnsRegistration.DevicePushIdentifier.Value.ShouldNotBe(otherPnsRegistration.DevicePushIdentifier.Value);
    }
}
