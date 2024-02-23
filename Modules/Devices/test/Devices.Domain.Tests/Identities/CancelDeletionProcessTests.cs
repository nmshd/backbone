using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class CancelDeletionProcessTests
{
    [Fact]
    public void Should_cancel_deletion_process_during_grace_period()
    {
        // Arrange
        var currentDate = SystemTime.UtcNow.Date;
        SystemTime.Set(currentDate);

        var activeIdentity = TestDataGenerator.CreateIdentity();
        var activeDevice = new Device(activeIdentity);
        activeIdentity.Devices.Add(activeDevice);

        activeIdentity.StartDeletionProcessAsOwner(activeDevice.Id);
        SystemTime.Set(currentDate.AddDays(5));

        // Act
        activeIdentity.CancelDeletionProcess(activeDevice.Id);

        // Assert
        var result = activeIdentity.DeletionProcesses.Where(dp => dp.Status == DeletionProcessStatus.Cancelled).ToList();
        result.Count.Should().Be(1);
    }

    [Fact]
    public void Should_throw_exception_when_cancelling_deletion_process_after_grace_period()
    {
        // Arrange
        var currentDate = SystemTime.UtcNow.Date;
        SystemTime.Set(currentDate);

        var activeIdentity = TestDataGenerator.CreateIdentity();
        var activeDevice = new Device(activeIdentity);
        activeIdentity.Devices.Add(activeDevice);

        activeIdentity.StartDeletionProcessAsOwner(activeDevice.Id);
        SystemTime.Set(currentDate.AddDays(35));

        // Act
        var result = () => activeIdentity.CancelDeletionProcess(activeDevice.Id);

        // Assert
        result.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessGracePeriodHasEnded");
    }

    [Fact]
    public void Should_throw_exception_when_deletion_process_does_not_exist()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentity();
        var activeDevice = new Device(activeIdentity);

        // Act
        var acting = () => activeIdentity.CancelDeletionProcess(activeDevice.Id);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.noDeletionProcessWithRequiredStatusExists");
    }
}
