using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.Utilities;
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
    public void Start_deletion_process_as_owner()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identityAddress = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        var activeIdentity = new Identity("", identityAddress, Array.Empty<byte>(), TierId.Generate(), 1);
        var asDevice = DeviceId.Parse("DVC");

        // Act
        activeIdentity.StartDeletionProcess(asDevice, new DummyHasher(new byte[] { 1, 2, 3 }));

        // Assert
        AssertDeletionProcessWasStarted(activeIdentity, "The deletion process was started by the owner.", new byte[] { 1, 2, 3 });
    }

    [Fact]
    public void Only_one_active_deletion_process_is_allowed_when_started_by_the_owner()
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

    [Fact]
    public void Start_deletion_process_as_support()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identityAddress = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        var activeIdentity = new Identity("", identityAddress, Array.Empty<byte>(), TierId.Generate(), 1);

        // Act
        activeIdentity.StartDeletionProcess(new DummyHasher(new byte[] { 1, 2, 3 }));

        // Assert
        AssertDeletionProcessWasStarted(activeIdentity, "The deletion process was started by a support employee.");
    }

    

    [Fact]
    public void Only_one_active_deletion_process_is_allowed_when_started_by_the_support()
    {
        // Arrange
        var identityAddress = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        var activeIdentity = new Identity("", identityAddress, Array.Empty<byte>(), TierId.Generate(), 1);

        activeIdentity.StartDeletionProcess();

        // Act
        var acting = () => activeIdentity.StartDeletionProcess();

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed");
    }

    private static void AssertDeletionProcessWasStarted(Identity activeIdentity, string message, byte[]? deviceIdHash = null)
    {
        activeIdentity.DeletionProcesses.Should().HaveCount(1);
        var deletionProcess = activeIdentity.DeletionProcesses[0];
        deletionProcess.Should().NotBeNull();

        deletionProcess.Id.Should().NotBeNull();
        deletionProcess.Id.Value.Should().HaveLength(20);

        deletionProcess.Status.Should().Be(DeletionProcessStatus.WaitingForApproval);
        deletionProcess.CreatedAt.Should().Be(SystemTime.UtcNow);

        deletionProcess.AuditLog.Should().HaveCount(1);

        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.Message.Should().Be(message);
        auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        if (deviceIdHash == null)
            auditLogEntry.DeviceIdHash.Should().BeNull();
        else
            auditLogEntry.DeviceIdHash.Should().BeEquivalentTo(deviceIdHash);

        auditLogEntry.OldStatus.Should().BeNull();
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
    }
}

file static class IdentityExtensions
{
    public static void StartDeletionProcess(this Identity identity, DeviceId asDevice)
    {
        identity.StartDeletionProcess(asDevice, A.Dummy<IHasher>());
    }

    public static void StartDeletionProcess(this Identity identity)
    {
        identity.StartDeletionProcess( A.Dummy<IHasher>());
    }
}
