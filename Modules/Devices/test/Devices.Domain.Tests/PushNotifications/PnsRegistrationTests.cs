using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Domain.Tests.PushNotifications;

public class PnsRegistrationTests
{
    [Fact]
    public void Generate_DevicePushIdentifier_while_instancing_PnsRegistration()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "value").Value;
        var time = DateTime.UtcNow;

        SystemTime.Set(time);

        // Act
        var pnsRegistration = new PnsRegistration(identityAddress, deviceId, pnsHandle, "appId", Environment.Development);

        // Assert
        pnsRegistration.IdentityAddress.Should().Be(identityAddress);
        pnsRegistration.DeviceId.Should().NotBeNull();
        pnsRegistration.DevicePushIdentifier.Should().NotBeNull();
        pnsRegistration.Handle.Should().Be(pnsHandle);
        pnsRegistration.UpdatedAt.Should().Be(time);
        pnsRegistration.AppId.Should().Be("appId");
        pnsRegistration.Environment.Should().Be(Environment.Development);
    }

    [Fact]
    public void Generate_DevicePushIdentifier_independent_of_PnsRegistration_constructor_parameters()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "value").Value;

        var otherPnsRegistration = new PnsRegistration(identityAddress, deviceId, pnsHandle, "appId", Environment.Development);

        // Act
        var pnsRegistration = new PnsRegistration(identityAddress, deviceId, pnsHandle, "appId", Environment.Development);

        // Assert
        pnsRegistration.DevicePushIdentifier.StringValue.Should().NotBe(otherPnsRegistration.DevicePushIdentifier.StringValue);
    }
}
