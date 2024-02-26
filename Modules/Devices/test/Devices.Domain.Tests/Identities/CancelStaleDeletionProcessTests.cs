using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class CancelStaleDeletionProcessTests
{
    [Fact]
    public void Cancel_stale_deletion_process_still_waiting_for_approval()
    {
        // Arrange
        var activeIdentity = new Identity("", IdentityAddress.Create([], "id1"), [], TierId.Generate(), 1);
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        // Act
        activeIdentity.CancelStaleDeletionProcess(activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval).Id);

        // Assert
        activeIdentity.Status.Should().Be(IdentityStatus.Active);

        var deletionProcess1 = activeIdentity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Canceled);
        deletionProcess1.AuditLog.Should().HaveCount(2);
        deletionProcess1.AuditLog[1].ProcessId.Should().Be(deletionProcess.Id);
        deletionProcess1.AuditLog[1].OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        deletionProcess1.AuditLog[1].NewStatus.Should().Be(DeletionProcessStatus.Canceled);
    }
}
