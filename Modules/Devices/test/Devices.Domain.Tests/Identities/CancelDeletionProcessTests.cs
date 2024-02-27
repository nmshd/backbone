using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class CancelDeletionProcessTests
{
    [Fact]
    public void Cancel_deletion_process()
    {
        // Arrange
        var currentDate = DateTime.Parse("2020-01-01");
        SystemTime.Set(currentDate);

        var activeIdentity = TestDataGenerator.CreateIdentity();
        var activeDevice = new Device(activeIdentity);
        activeIdentity.Devices.Add(activeDevice);

        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(activeDevice.Id);
        SystemTime.Set(DateTime.Parse("2020-01-02"));

        // Act
        activeIdentity.CancelDeletionProcess(activeDevice.Id);

        // Assert
        deletionProcess.Status.Should().Be(DeletionProcessStatus.Cancelled);
    }

    [Fact]
    public void Throws_when_cancelling_deletion_process_after_grace_period()
    {
        // Arrange
        var currentDate = DateTime.Parse("2020-01-01");
        SystemTime.Set(currentDate);

        var activeIdentity = TestDataGenerator.CreateIdentity();
        var activeDevice = new Device(activeIdentity);
        activeIdentity.Devices.Add(activeDevice);

        activeIdentity.StartDeletionProcessAsOwner(activeDevice.Id);
        SystemTime.Set(DateTime.Parse("2020-02-02"));

        // Act
        var result = () => activeIdentity.CancelDeletionProcess(activeDevice.Id);

        // Assert
        result.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessGracePeriodHasEnded");
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
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
