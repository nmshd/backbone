using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
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

        var identity = CreateIdentityWithApprovedDeletionProcess();
        SystemTime.Set(DateTime.Parse("2020-01-02"));

        // Act
        var deletionProcess = identity.CancelDeletionProcess(identity.DeletionProcesses[0].Id, identity.Devices[0].Id);

        // Assert
        identity.TierIdBeforeDeletion.Should().Be(null);
        identity.Status.Should().Be(IdentityStatus.Active);
        deletionProcess.Status.Should().Be(DeletionProcessStatus.Cancelled);
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        identity.Devices.Add(new Device(identity));
        var deviceId = identity.Devices[0].Id;
        var deletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;

        // Act
        var acting = () => identity.CancelDeletionProcess(deletionProcessId, deviceId);

        // Assert
        acting.Should().Throw<DomainException>().Which.Message.Should().Contain("IdentityDeletionProcess");
    }

    [Fact]
    public void Throws_when_deletion_process_is_in_wrong_status()
    {
        // Arrange
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();
        identity.Devices.Add(new Device(identity));

        // Act
        var acting = () => identity.CancelDeletionProcess(identity.DeletionProcesses[0].Id, identity.Devices[0].Id);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.noDeletionProcessWithRequiredStatusExists");
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.Should().HaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.OldStatus.Should().Be(DeletionProcessStatus.Approved);
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Cancelled);
    }

    private static Identity CreateIdentityWithApprovedDeletionProcess()
    {
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);
        identity.Devices.Add(device);
        identity.StartDeletionProcessAsOwner(device.Id);
        return identity;
    }

    private static Identity CreateIdentityWithDeletionProcessWaitingForApproval()
    {
        var identity = TestDataGenerator.CreateIdentity();
        Hasher.SetHasher(new DummyHasher([1, 2, 3]));
        identity.StartDeletionProcessAsSupport();
        return identity;
    }
}
