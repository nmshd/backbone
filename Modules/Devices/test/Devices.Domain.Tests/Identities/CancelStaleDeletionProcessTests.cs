using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
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

        SystemTime.Set(new DateTime(2024, 05, 06, 09, 50, 06));

        // Act
        identity.CancelStaleDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id);

        // Assert
        identity.Status.Should().Be(IdentityStatus.Active);

        deletionProcess.Status.Should().Be(DeletionProcessStatus.Cancelled);
        deletionProcess.CancelledAt.Should().Be("2024-05-06T09:50:06");
    }

    [Fact]
    public void Canceling_stale_deletion_process_creates_audit_log_when_executed()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        // Act
        identity.CancelStaleDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id);

        // Assert
        var canceledDeletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Cancelled)!;

        canceledDeletionProcess.AuditLog.Should().HaveCount(2); // count 2 because the first one was creation of a deletion process
        canceledDeletionProcess.AuditLog[1].ProcessId.Should().Be(deletionProcess.Id);
        canceledDeletionProcess.AuditLog[1].OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        canceledDeletionProcess.AuditLog[1].NewStatus.Should().Be(DeletionProcessStatus.Cancelled);
    }
}
