using Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.PushNotifications;

public class CreateRandomIdentifierForPushNotificationRegistration
{
    [Fact]
    public void Generate_random_identifier()
    {
        // Arrange

        // Act
        var devicePushIdentifier = DevicePushIdentifier.New();

        // Assert
        devicePushIdentifier.Should().NotBeNull();
        // pametniji testovi?
    }

    // [Fact]
    // public void Generate_random_identifier_while_instancing_PnsRegistration_class()
    // {
    //     // Arrange
    //     var randomIdentityAddress = CreateRandomIdentityAddress();
    //     var randomDeviceId = CreateRandomDeviceId();
    //     var pnsHandle = PnsHandle.Parse("value", PushNotificationPlatform.Fcm).Value;
    //
    //     // var identifierTestValue = DevicePushIdentifier.Create(randomDeviceId);
    //     var identifierTestValue = randomDeviceId + "-" + DevicePushIdentifierSuffixGenerator.GenerateSuffixUtf8();
    //
    //     // Act
    //     var pnsRegistration = new PnsRegistration(randomIdentityAddress, randomDeviceId, pnsHandle, "appId", Environment.Development);
    //
    //     // Assert
    //     pnsRegistration.IdentityAddress.Should().BeEquivalentTo(randomIdentityAddress);
    //     pnsRegistration.DeviceId.Should().BeEquivalentTo(randomDeviceId);
    //     pnsRegistration.DevicePushIdentifier.Value.Should().BeEquivalentTo(identifierTestValue);
    //     pnsRegistration.Handle.Should().BeEquivalentTo(pnsHandle);
    //     pnsRegistration.AppId.Should().BeEquivalentTo("appId");
    //     pnsRegistration.Environment.Should().Be(Environment.Development);
    // }
}
