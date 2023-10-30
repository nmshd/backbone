using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using FluentAssertions;
using Xunit;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;

public class CreateRandomIdentifierForPushNotificationRegistration
{
    [Fact]
    public void Generate_random_identifier()
    {
        // Arrange
        var randomDeviceId = TestDataGenerator.CreateRandomDeviceId();
        var identifierTestValue = randomDeviceId + "-" + RandomIdentifierHasher.HashUtf8("seed");

        // Act
        var randomIdentifier = DevicePushIdentifier.Create(randomDeviceId);

        // Assert
        randomIdentifier.Value.Should().BeEquivalentTo(identifierTestValue);
    }

    [Fact]
    public void Generate_random_identifier_while_instancing_PnsRegistration_class()
    {
        // Arrange
        var randomIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var randomDeviceId = TestDataGenerator.CreateRandomDeviceId();
        var pnsHandle = PnsHandle.Parse("value", PushNotificationPlatform.Fcm).Value;

        // var identifierTestValue = DevicePushIdentifier.Create(randomDeviceId);
        var identifierTestValue = randomDeviceId + "-" + RandomIdentifierHasher.HashUtf8("seed");

        // Act
        var pnsRegistration = new PnsRegistration(randomIdentityAddress, randomDeviceId, pnsHandle, "appId", Environment.Development);

        // Assert
        pnsRegistration.IdentityAddress.Should().BeEquivalentTo(randomIdentityAddress);
        pnsRegistration.DeviceId.Should().BeEquivalentTo(randomDeviceId);
        pnsRegistration.DevicePushIdentifier.Value.Should().BeEquivalentTo(identifierTestValue);
        pnsRegistration.Handle.Should().BeEquivalentTo(pnsHandle);
        pnsRegistration.AppId.Should().BeEquivalentTo("appId");
        pnsRegistration.Environment.Should().Be(Environment.Development);
    }
}
