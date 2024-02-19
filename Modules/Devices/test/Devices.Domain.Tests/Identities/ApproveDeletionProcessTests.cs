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
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        identity.ApproveDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id, DeviceId.Parse("DVC"));

        // Assert
        identity.Status.Should().Be(IdentityStatus.ToBeDeleted);
        identity.DeletionGracePeriodEndsAt.Should().Be(DateTime.Parse("2000-01-31"));
        identity.TierId.Should().Be(Tier.QUEUED_FOR_DELETION.Id);
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var identity = CreateIdentity();
        var identityDeletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;

        // Act
        var acting = () => identity.ApproveDeletionProcess(identityDeletionProcessId, DeviceId.Parse("DVC"));

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
        var acting = () => identity.ApproveDeletionProcess(deletionProcess.Id, DeviceId.Parse("DVC"));

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
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Approved);
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        return new Identity("", address, Array.Empty<byte>(), TierId.Generate(), 1);
    }

    private static Identity CreateIdentityWithDeletionProcessWaitingForApproval()
    {
        var identity = CreateIdentity();
        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));
        identity.StartDeletionProcessAsSupport();
        return identity;
    }
}
