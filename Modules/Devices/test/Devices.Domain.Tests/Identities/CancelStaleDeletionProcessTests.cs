using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class CancelStaleDeletionProcessTests
{
    [Fact]
    public void Cancel_deletion_process_that_is_past_due_approval()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        var utcNow = DateTime.Parse("2020-01-01");
        SystemTime.Set(utcNow);

        // Act
        identity.CancelStaleDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id);

        // Assert
        identity.Status.Should().Be(IdentityStatus.Active);

        deletionProcess.Status.Should().Be(DeletionProcessStatus.Cancelled);
        deletionProcess.CancelledAt.Should().Be(utcNow);
    }

    [Fact]
    public void Canceling_stale_deletion_process_creates_audit_log_entry_when_executed()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        var deletionProcess = identity.CancelStaleDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id);

        // Assert
        deletionProcess.AuditLog.Should().HaveCount(2); // count 2 because the first one was creation of the deletion process
        deletionProcess.AuditLog[1].ProcessId.Should().Be(deletionProcess.Id);
        deletionProcess.AuditLog[1].OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        deletionProcess.AuditLog[1].NewStatus.Should().Be(DeletionProcessStatus.Cancelled);
    }
}
