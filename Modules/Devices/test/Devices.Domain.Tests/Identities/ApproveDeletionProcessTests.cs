using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class ApproveDeletionProcessTests
{
    [Fact]
    public void Approve_deletion_process_waiting_for_approval()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();
        identity.Devices.Add(new Device(identity));
        var deviceId = identity.Devices[0].Id;

        // Act
        identity.ApproveDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id, deviceId);

        // Assert
        identity.Status.Should().Be(IdentityStatus.ToBeDeleted);
        identity.DeletionGracePeriodEndsAt.Should().Be(DateTime.Parse("2000-01-31"));
        identity.TierId.Should().Be(Tier.QUEUED_FOR_DELETION.Id);
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    [Fact]
    public void Throws_when_device_not_owned_by_identity()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01"));
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        var acting = () => identity.ApproveDeletionProcess(identity.DeletionProcesses[0].Id, DeviceId.Parse("DVC"));

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var identity = CreateIdentity();
        identity.Devices.Add(new Device(identity));
        var deviceId = identity.Devices[0].Id;
        var deletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;

        // Act
        var acting = () => identity.ApproveDeletionProcess(deletionProcessId, deviceId);

        // Assert
        acting.Should().Throw<DomainException>().Which.Message.Should().Contain("IdentityDeletionProcess");
    }

    [Fact]
    public void Throws_when_deletion_process_is_not_in_status_waiting_for_approval()
    {
        // Arrange
        var identity = CreateIdentity();
        identity.Devices.Add(new Device(identity));
        var deviceId = identity.Devices[0].Id;
        var deletionProcess = identity.StartDeletionProcessAsOwner(deviceId);

        // Act
        var acting = () => identity.ApproveDeletionProcess(deletionProcess.Id, deviceId);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessNotInRequiredStatus");
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.Should().HaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Approved);
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create([], "id1");
        return new Identity("", address, [], TierId.Generate(), 1);
    }

    private static Identity CreateIdentityWithDeletionProcessWaitingForApproval()
    {
        var identity = CreateIdentity();
        Hasher.SetHasher(new DummyHasher([1, 2, 3]));
        identity.StartDeletionProcessAsSupport();
        return identity;
    }
}
