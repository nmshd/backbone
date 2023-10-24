using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using FakeItEasy;
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
        var identityAddress = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        var activeIdentity = new Identity("", identityAddress, Array.Empty<byte>(), TierId.Generate(), 1);
        var asDevice = DeviceId.Parse("DVC");

        // Act
        activeIdentity.StartDeletionProcess(asDevice);

        // Assert
        activeIdentity.DeletionProcesses.Should().HaveCount(1);
        var deletionProcess = activeIdentity.DeletionProcesses[0];
        deletionProcess.Should().NotBeNull();

        deletionProcess.Id.Should().NotBeNull();
        deletionProcess.Id.Value.Should().HaveLength(20);

        deletionProcess.Status.Should().Be(DeletionProcessStatus.WaitingForApproval);
        deletionProcess.CreatedAt.Should().Be(SystemTime.UtcNow);
    }

    [Fact]
    public void The_deletion_process_has_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identityAddress = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        var activeIdentity = new Identity("", identityAddress, Array.Empty<byte>(), TierId.Generate(), 1);
        var asDevice = DeviceId.Parse("DVC");

        // Act
        activeIdentity.StartDeletionProcess(asDevice, new DummyHasher(new byte[] { 1, 2, 3 }));

        // Assert
        var deletionProcess = activeIdentity.DeletionProcesses[0];

        deletionProcess.AuditLog.Should().HaveCount(1);

        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.Message.Should().Be("Started deletion process.");
        auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.DeviceIdHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.Should().BeNull();
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
    }

    [Fact]
    public void Only_one_active_deletion_process_is_allowed()
    {
        // Arrange
        var identityAddress = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        var activeIdentity = new Identity("", identityAddress, Array.Empty<byte>(), TierId.Generate(), 1);
        var asDevice = DeviceId.Parse("DVC");

        activeIdentity.StartDeletionProcess(asDevice);

        // Act
        var acting = () => activeIdentity.StartDeletionProcess(asDevice);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed");
    }

    // todo: start process by support
}

file static class IdentityExtensions
{
    public static void StartDeletionProcess(this Identity identity, DeviceId asDevice)
    {
        identity.StartDeletionProcess(asDevice, A.Dummy<IHasher>());
    }
}
