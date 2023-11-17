using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.Utilities;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class StartDeletionProcessTests : IDisposable
{
    [Fact]
    public void Start_deletion_process_as_owner()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        var asDevice = DeviceId.Parse("DVC");

        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));

        // Act
        activeIdentity.StartDeletionProcess(asDevice);

        // Assert
        AssertDeletionProcessWasStarted(activeIdentity);
        var deletionProcess = activeIdentity.DeletionProcesses[0];
        deletionProcess.Status.Should().Be(DeletionProcessStatus.Approved);
        deletionProcess.ApprovedAt.Should().Be(SystemTime.UtcNow);
        deletionProcess.ApprovedByDevice.Should().Be(asDevice);

        AssertAuditLogEntryWasCreated(deletionProcess);
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.Message.Should().Be("The deletion process was started by the owner. It was automatically approved.");
        auditLogEntry.DeviceIdHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Approved);
    }

    [Fact]
    public void Only_one_active_deletion_process_is_allowed_when_started_by_the_owner()
    {
        // Arrange
        var activeIdentity = CreateIdentity();
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
        var activeIdentity = CreateIdentity();

        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));

        // Act
        activeIdentity.StartDeletionProcess();

        // Assert
        AssertDeletionProcessWasStarted(activeIdentity);
        var deletionProcess = activeIdentity.DeletionProcesses[0];
        deletionProcess.Status.Should().Be(DeletionProcessStatus.WaitingForApproval);
        deletionProcess.ApprovedAt.Should().BeNull();
        deletionProcess.ApprovedByDevice.Should().BeNull();

        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.Message.Should().Be("The deletion process was started by a support employee.");
        auditLogEntry.DeviceIdHash.Should().BeNull();
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
    }



    [Fact]
    public void Only_one_active_deletion_process_is_allowed_when_started_by_the_support()
    {
        // Arrange
        var activeIdentity = CreateIdentity();

        activeIdentity.StartDeletionProcess();

        // Act
        var acting = () => activeIdentity.StartDeletionProcess();

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed");
    }

    private static void AssertDeletionProcessWasStarted(Identity activeIdentity)
    {
        activeIdentity.DeletionProcesses.Should().HaveCount(1);
        var deletionProcess = activeIdentity.DeletionProcesses[0];
        deletionProcess.Should().NotBeNull();

        deletionProcess.Id.Should().NotBeNull();
        deletionProcess.Id.Value.Should().HaveLength(20);

        deletionProcess.CreatedAt.Should().Be(SystemTime.UtcNow);

        deletionProcess.AuditLog.Should().HaveCount(1);
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.Should().BeNull();
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        return new Identity("", address, Array.Empty<byte>(), TierId.Generate(), 1);
    }

    public void Dispose()
    {
        Hasher.Reset();
    }
}
