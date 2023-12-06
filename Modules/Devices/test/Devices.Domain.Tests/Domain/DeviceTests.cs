using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
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
        var device = new Device(identity);

        device.User = new ApplicationUser(identity, device.Id);

        // Act
        device.MarkAsDeleted(device.Id);

        // Assert
        device.DeletedAt.Should().NotBeNull();
        device.DeletedByDevice.Should().NotBeNull();
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
        var action = () => device.MarkAsDeleted(device.Id);

        // Assert
        var domainException = action.Should().Throw<DomainException>().Which;
        domainException.Code.Should().Be("error.platform.validation.device.deviceCannotBeDeleted");
    }

    [Fact]
    public void Deleting_a_device_not_owned_by_the_current_identity_is_not_possible()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        device.User = new ApplicationUser(identity, device.Id);

        // Act
        var action = () => device.MarkAsDeleted(DeviceId.New());

        // Assert
        var domainException = action.Should().Throw<DomainException>().Which;
        domainException.Code.Should().Be("error.platform.validation.device.deviceCannotBeDeleted");
    }
}
