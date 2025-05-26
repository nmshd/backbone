using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class ApproveDeletionProcessTests : AbstractTestsBase
{
    [Fact]
    public void Approve_deletion_process_waiting_for_approval()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();
        identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
        var deviceId = identity.Devices[0].Id;

        // Act
        identity.ApproveDeletionProcess(identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!.Id, deviceId);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.ToBeDeleted);
        identity.DeletionGracePeriodEndsAt.ShouldBe(DateTime.Parse("2000-01-15"));
        identity.TierId.ShouldBe(Tier.QUEUED_FOR_DELETION.Id);
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    [Fact]
    public void Throws_when_device_not_owned_by_identity()
    {
        // Arrange
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        var acting = () => identity.ApproveDeletionProcess(identity.DeletionProcesses[0].Id, DeviceId.Parse("DVC"));

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.recordNotFound", "Device");
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var identity = CreateIdentity();
        identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
        var deviceId = identity.Devices[0].Id;
        var deletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;

        // Act
        var acting = () => identity.ApproveDeletionProcess(deletionProcessId, deviceId);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.recordNotFound", "IdentityDeletionProcess");
    }

    [Fact]
    public void Throws_when_deletion_process_is_not_in_status_waiting_for_approval()
    {
        // Arrange
        var identity = CreateIdentity();
        identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
        var deviceId = identity.Devices[0].Id;
        var deletionProcess = identity.StartDeletionProcessAsOwner(deviceId);

        // Act
        var acting = () => identity.ApproveDeletionProcess(deletionProcess.Id, deviceId);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus", "WaitingForApproval");
    }

    [Fact]
    public void Raises_domain_events()
    {
        //Arrange
        var activeIdentity = TestDataGenerator.CreateIdentity();
        var activeDevice = activeIdentity.Devices[0];
        var tierBeforeDeletion = activeIdentity.TierId;
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        //Act
        activeIdentity.ApproveDeletionProcess(deletionProcess.Id, activeDevice.Id);

        //Assert
        var (tierOfIdentityChangedDomainEvent, identityToBeDeletedDomainEvent) = activeIdentity.ShouldHaveDomainEvents<TierOfIdentityChangedDomainEvent, IdentityToBeDeletedDomainEvent>();

        tierOfIdentityChangedDomainEvent.IdentityAddress.ShouldBe(activeIdentity.Address);
        tierOfIdentityChangedDomainEvent.OldTierId.ShouldBe(tierBeforeDeletion);
        tierOfIdentityChangedDomainEvent.NewTierId.ShouldBe(Tier.QUEUED_FOR_DELETION.Id);

        identityToBeDeletedDomainEvent.IdentityAddress.ShouldBe(activeIdentity.Address);
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.ShouldHaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.ProcessId.ShouldBe(deletionProcess.Id);
        auditLogEntry.CreatedAt.ShouldBe(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.ShouldBeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.ShouldBe(DeletionProcessStatus.WaitingForApproval);
        auditLogEntry.NewStatus.ShouldBe(DeletionProcessStatus.Approved);
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create([], "prod.enmeshed.eu");
        return new Identity("", address, [], TierId.Generate(), 1, CommunicationLanguage.DEFAULT_LANGUAGE);
    }

    private static Identity CreateIdentityWithDeletionProcessWaitingForApproval()
    {
        var identity = CreateIdentity();
        Hasher.SetHasher(new DummyHasher([1, 2, 3]));
        identity.StartDeletionProcessAsSupport();
        return identity;
    }

    public override void Dispose()
    {
        base.Dispose();
        Hasher.Reset();
    }
}
