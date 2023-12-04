using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Domain;
public class DeviceTests
{
    [Fact]
    public void Device_can_be_deleted()
    {
        // Arrange
        var identity = UnitTestTools.Data.TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        device.User = new ApplicationUser(identity, device.Id);

        // Act
        device.MarkAsDeleted(DeviceId.New());

        // Assert
        device.DeletedAt.Should().NotBeNull();
        device.DeletedByDevice.Should().NotBeNull();
    }

    [Fact]
    public void Test_deleting_an_onboarded_device_which_throws_DomainException()
    {
        // Arrange
        var identity = UnitTestTools.Data.TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        device.User = new ApplicationUser(identity, device.Id);
        device.User.LoginOccurred();

        // Act
        var action = () => device.MarkAsDeleted(device.Id);

        // Assert
        action.Should().Throw<DomainException>();
    }
}
