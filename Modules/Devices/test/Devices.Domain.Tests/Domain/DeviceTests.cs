using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Domain;

public class DeviceTests
{
    [Fact]
    public void IsOnboarded_returns_false_if_user_has_never_logged_in_before()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        device.User = new ApplicationUser(identity, device.Id);

        // Act
        var isOnboarded = device.IsOnboarded;

        // Assert
        isOnboarded.Should().BeFalse();
    }

    [Fact]
    public void IsOnboarded_returns_true_if_user_has_been_used_to_login()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        device.User = new ApplicationUser(identity, device.Id);
        device.User.LoginOccurred();

        // Act
        var isOnboarded = device.IsOnboarded;

        // Assert
        isOnboarded.Should().BeTrue();
    }

    [Fact]
    public void An_unOnboarded_device_can_be_deleted()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        var activeDevice = CreateOnboardedDevice(identity);
        var unOnboardedDevice = CreateUnonboardedDevice(identity);

        // Act
        unOnboardedDevice.MarkAsDeleted(activeDevice.Id, identity.Address);

        // Assert
        unOnboardedDevice.DeletedAt.Should().NotBeNull();
        unOnboardedDevice.DeletedByDevice.Should().Be(activeDevice.Id);
    }

    [Fact]
    public void An_onboarded_device_cannot_be_deleted()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        var activeDevice = CreateOnboardedDevice(identity);
        var onboardedDevice = CreateOnboardedDevice(identity);

        // Act
        var action = () => onboardedDevice.MarkAsDeleted(activeDevice.Id, identity.Address);

        // Assert
        var domainException = action.Should().Throw<DomainException>().Which;
        domainException.Code.Should().Be("error.platform.validation.device.deviceCannotBeDeleted");
    }

    [Fact]
    public void A_device_not_owned_by_active_identity_cannot_be_deleted()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentity();
        var otherIdentity = TestDataGenerator.CreateIdentity();

        var activeDevice = CreateOnboardedDevice(activeIdentity);
        var unOnboardedDeviceOfOtherIdentity = CreateUnonboardedDevice(otherIdentity);

        // Act
        var action = () => unOnboardedDeviceOfOtherIdentity.MarkAsDeleted(activeDevice.Id, activeIdentity.Address);

        // Assert
        var domainException = action.Should().Throw<DomainException>().Which;
        domainException.Code.Should().Be("error.platform.validation.device.deviceCannotBeDeleted");
    }

    private static Device CreateUnonboardedDevice(Identity identity)
    {
        var activeDevice = new Device(identity);
        activeDevice.User = new ApplicationUser(identity, activeDevice.Id);
        return activeDevice;
    }

    private static Device CreateOnboardedDevice(Identity identity)
    {
        var activeDevice = new Device(identity);
        activeDevice.User = new ApplicationUser(identity, activeDevice.Id);
        activeDevice.User.LoginOccurred();
        return activeDevice;
    }
}
