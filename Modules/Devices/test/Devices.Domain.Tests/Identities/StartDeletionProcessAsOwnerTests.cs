using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class StartDeletionProcessAsOwnerTests : IDisposable
{
    [Fact]
    public void Start_deletion_process()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();
        var device = DeviceId.Parse("DVC");

        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));

        // Act
        var deletionProcess = identity.StartDeletionProcessAsOwner(device);

        // Assert
        identity.DeletionGracePeriodEndsAt.Should().Be(DateTime.Parse("2000-01-31"));

        AssertDeletionProcessWasStarted(identity);
        deletionProcess.Status.Should().Be(DeletionProcessStatus.Approved);
        deletionProcess.ApprovedAt.Should().Be(SystemTime.UtcNow);
        deletionProcess.ApprovedByDevice.Should().Be(device);
        deletionProcess.GracePeriodEndsAt.Should().Be(DateTime.Parse("2000-01-31"));

        AssertAuditLogEntryWasCreated(deletionProcess);
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.Message.Should().Be("The deletion process was started by the owner. It was automatically approved.");
        auditLogEntry.DeviceIdHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Approved);
    }

    [Fact]
    public void Only_one_active_deletion_process_is_allowed_when_started()
    {
        // Arrange
        var identity = CreateIdentity();
        var device = DeviceId.Parse("DVC");

        identity.StartDeletionProcessAsOwner(device);

        // Act
        var acting = () => identity.StartDeletionProcessAsOwner(device);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed");
    }

    private static void AssertDeletionProcessWasStarted(Identity identity)
    {
        identity.DeletionProcesses.Should().HaveCount(1);
        var deletionProcess = identity.DeletionProcesses[0];
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
