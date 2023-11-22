using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Domain.Tests.PushNotifications;

public class PnsRegistrationTests
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
        pnsRegistration.IdentityAddress.Should().Be(identityAddress);
        pnsRegistration.DeviceId.Should().NotBeNull();
        pnsRegistration.DevicePushIdentifier.Should().NotBeNull();
        pnsRegistration.Handle.Should().Be(pnsHandle);
        pnsRegistration.UpdatedAt.Should().Be(SystemTime.UtcNow);
        pnsRegistration.AppId.Should().Be("someAppId");
        pnsRegistration.Environment.Should().Be(PushEnvironment.Development);
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
        pnsRegistration.DevicePushIdentifier.StringValue.Should().NotBe(otherPnsRegistration.DevicePushIdentifier.StringValue);
    }
}
