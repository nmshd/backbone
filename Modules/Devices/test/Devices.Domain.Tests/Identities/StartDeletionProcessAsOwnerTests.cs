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
    public void Dispose()
    {
        Hasher.Reset();
    }

    [Fact]
    public void Start_deletion_process()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        var activeDevice = new Device(activeIdentity);
        activeIdentity.Devices.Add(activeDevice);

        Hasher.SetHasher(new DummyHasher([1, 2, 3]));

        // Act
        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(activeDevice.Id);

        // Assert
        activeIdentity.DeletionGracePeriodEndsAt.Should().Be(DateTime.Parse("2000-01-15"));
        activeIdentity.TierId.Value.Should().Be(Tier.QUEUED_FOR_DELETION.Id.Value);
        activeIdentity.Status.Should().Be(IdentityStatus.ToBeDeleted);

        AssertDeletionProcessWasStarted(activeIdentity);
        deletionProcess.Status.Should().Be(DeletionProcessStatus.Approved);
        deletionProcess.ApprovedAt.Should().Be(SystemTime.UtcNow);
        deletionProcess.ApprovedByDevice.Should().Be(activeDevice.Id);
        deletionProcess.GracePeriodEndsAt.Should().Be(DateTime.Parse("2000-01-15"));

        AssertAuditLogEntryWasCreated(deletionProcess);
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.Message.Should().Be("The deletion process was started by the owner. It was automatically approved.");
        auditLogEntry.DeviceIdHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Approved);
    }

    [Fact]
    public void Throws_when_device_not_owned_by_identity()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01"));
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        identity.Devices.Add(device);

        // Act
        var acting = () => identity.StartDeletionProcessAsOwner(DeviceId.Parse("DVC"));

        // Assert
        var exception = acting.Should().Throw<DomainException>().Which;
        exception.Code.Should().Be("error.platform.recordNotFound");
        exception.Message.Should().Contain("Device");
    }

    [Fact]
    public void Only_one_active_deletion_process_is_allowed_when_started()
    {
        // Arrange
        var activeIdentity = CreateIdentity();
        var activeDevice = new Device(activeIdentity);
        activeIdentity.Devices.Add(activeDevice);

        activeIdentity.StartDeletionProcessAsOwner(activeDevice.Id);

        // Act
        var acting = () => activeIdentity.StartDeletionProcessAsOwner(activeDevice.Id);

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
}
