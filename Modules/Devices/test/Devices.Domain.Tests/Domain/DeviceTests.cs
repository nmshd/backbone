using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Domain;

public class DeviceTests
{
    [Fact]
    public void Device_is_onboarded()
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
    public void Device_can_be_deleted()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        var onboardedDevice = new Device(identity);
        onboardedDevice.User = new ApplicationUser(identity, onboardedDevice.Id);
        onboardedDevice.User.LoginOccurred();

        var unOnboardedDevice = new Device(identity);
        unOnboardedDevice.User = new ApplicationUser(identity, unOnboardedDevice.Id);

        // Act
        unOnboardedDevice.MarkAsDeleted(onboardedDevice.Id, identity.Address);

        // Assert
        unOnboardedDevice.DeletedAt.Should().NotBeNull();
        unOnboardedDevice.DeletedByDevice.Should().Be(onboardedDevice.Id);
    }

    [Fact]
    public void Deleting_an_onboarded_device_is_not_possible()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        device.User = new ApplicationUser(identity, device.Id);
        device.User.LoginOccurred();

        // Act
        var action = () => device.MarkAsDeleted(device.Id, identity.Address);

        // Assert
        var domainException = action.Should().Throw<DomainException>().Which;
        domainException.Code.Should().Be("error.platform.validation.device.deviceCannotBeDeleted");
    }

    [Fact]
    public void Deleting_a_device_by_itself_is_not_possible()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        device.User = new ApplicationUser(identity, device.Id);
        device.User.LoginOccurred();

        // Act
        var action = () => device.MarkAsDeleted(device.Id, identity.Address);

        // Assert
        var domainException = action.Should().Throw<DomainException>().Which;
        domainException.Code.Should().Be("error.platform.validation.device.deviceCannotBeDeleted");
    }
}
