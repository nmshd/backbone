using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Domain.Tests.PushNotifications;

public class CreateRandomIdentifierForPushNotificationRegistration
{
    [Fact]
    public void Generate_DevicePushIdentifier_while_instancing_PnsRegistration()
    {
        // Arrange
        var randomIdentityAddress = CreateRandomIdentityAddress();
        var randomDeviceId = CreateRandomDeviceId();
        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "value").Value;

        // Act
        var pnsRegistration = new PnsRegistration(randomIdentityAddress, randomDeviceId, pnsHandle, "appId", Environment.Development);

        // Assert
        pnsRegistration.DevicePushIdentifier.StringValue[..3].Should().Be("DPI");
        pnsRegistration.DevicePushIdentifier.StringValue.Length.Should().Be(20);
    }

    [Fact]
    public void Generate_DevicePushIdentifier_independent_of_PnsRegistration_constructor_parameters()
    {
        // Arrange
        var randomIdentityAddress = CreateRandomIdentityAddress();
        var randomDeviceId = CreateRandomDeviceId();
        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "value").Value;

        var otherPnsRegistration = new PnsRegistration(randomIdentityAddress, randomDeviceId, pnsHandle, "appId", Environment.Development);

        // Act
        var pnsRegistration = new PnsRegistration(randomIdentityAddress, randomDeviceId, pnsHandle, "appId", Environment.Development);

        // Assert
        pnsRegistration.DevicePushIdentifier.StringValue.Should().NotBe(otherPnsRegistration.DevicePushIdentifier.StringValue);
    }
}
