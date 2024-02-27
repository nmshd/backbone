using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class RejectDeletionProcessTests
{
    [Fact]
    public void Reject_deletion_process_waiting_for_approval()
    {
        // Arrange
        SystemTime.Set(SystemTime.UtcNow);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();
        identity.Devices.Add(new Device(identity));
        var deviceId = identity.Devices[0].Id;

        // Act
        identity.RejectDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id, deviceId);

        // Assert
        identity.Status.Should().Be(IdentityStatus.Active);

        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Rejected)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    [Fact]
    public void Throws_when_device_not_owned_by_identity()
    {
        // Arrange
        SystemTime.Set(SystemTime.UtcNow);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        var acting = () => identity.RejectDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id, DeviceId.Parse("DVC"));

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var identity = CreateIdentity();
        var identityDeletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;

        // Act
        var acting = () => identity.RejectDeletionProcess(identityDeletionProcessId, DeviceId.Parse("DVC"));

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Throws_when_deletion_process_is_not_in_status_waiting_for_approval()
    {
        // Arrange
        var identity = CreateIdentity();
        var deletionProcess = identity.StartDeletionProcessAsOwner(DeviceId.Parse("DVC"));

        // Act
        var acting = () => identity.RejectDeletionProcess(deletionProcess.Id, DeviceId.Parse("DVC"));

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.noDeletionProcessWithRequiredStatusExists");
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.Should().HaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Rejected);
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
