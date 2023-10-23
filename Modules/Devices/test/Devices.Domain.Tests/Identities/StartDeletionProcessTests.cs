using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class StartDeletionProcessTests
{
    [Fact]
    public void Adds_a_new_deletion_process()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = new Identity("", IdentityAddress.Create(Array.Empty<byte>(), "id1"), Array.Empty<byte>(), TierId.Generate(), 1);
        var asDevice = DeviceId.Parse("DVC");

        // Act
        identity.StartDeletionProcess(asDevice);

        // Assert
        identity.DeletionProcesses.Should().HaveCount(1);
        var deletionProcess = identity.DeletionProcesses[0];
        deletionProcess.Should().NotBeNull();

        deletionProcess.Id.Should().NotBeNull();
        deletionProcess.Id.Value.Should().HaveLength(20);

        deletionProcess.CreatedBy.Should().Be(identity.Address);
        deletionProcess.Status.Should().Be(DeletionProcessStatus.WaitingForApproval);
        deletionProcess.CreatedAt.Should().Be(SystemTime.UtcNow);
        deletionProcess.CreatedByDevice.Should().Be(asDevice);
    }

    [Fact]
    public void The_deletion_process_has_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = new Identity("", IdentityAddress.Create(Array.Empty<byte>(), "id1"), Array.Empty<byte>(), TierId.Generate(), 1);
        var asDevice = DeviceId.Parse("DVC");

        // Act
        identity.StartDeletionProcess(asDevice);

        // Assert
        var deletionProcess = identity.DeletionProcesses[0];

        deletionProcess.AuditLog.Should().HaveCount(1);

        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.Message.Should().NotBe("Created"); // TODO: real message
        auditLogEntry.IdentityAddressHash.Should().NotBeEmpty();
        auditLogEntry.DeviceIdHash.Should().NotBeEmpty();
        auditLogEntry.OldStatus.Should().NotBeNull();
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
    }

    public void OnlyOneActiveProcess()MinimumNumberOfApprovals
}
