using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class CancelStaleDeletionProcessTests
{
    [Fact]
    public void Cancel_stale_deletion_process_still_waiting_for_approval()
    {
        // Arrange
        var identity = new Identity("", IdentityAddress.Create([], "id1"), [], TierId.Generate(), 1);
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        // Act
        identity.CancelStaleDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id);

        // Assert
        identity.Status.Should().Be(IdentityStatus.Active);

        var canceledDeletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Canceled)!;

        canceledDeletionProcess.AuditLog.Should().HaveCount(2); // count 2 because the first one was creation of a deletion process
        canceledDeletionProcess.AuditLog[1].ProcessId.Should().Be(deletionProcess.Id);
        canceledDeletionProcess.AuditLog[1].OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        canceledDeletionProcess.AuditLog[1].NewStatus.Should().Be(DeletionProcessStatus.Canceled);
    }
}
