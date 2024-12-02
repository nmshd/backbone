using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;

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
        identity.Status.Should().Be(IdentityStatus.ToBeDeleted);
        identity.DeletionGracePeriodEndsAt.Should().Be(DateTime.Parse("2000-01-15"));
        identity.TierId.Should().Be(Tier.QUEUED_FOR_DELETION.Id);
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
        var exception = acting.Should().Throw<DomainException>().Which;

        // Assert
        exception.Code.Should().Be("error.platform.recordNotFound");
        exception.Message.Should().Contain("Device");
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
        var exception = acting.Should().Throw<DomainException>().Which;

        // Assert
        exception.Code.Should().Be("error.platform.recordNotFound");
        exception.Message.Should().Contain("IdentityDeletionProcess");
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
        var exception = acting.Should().Throw<DomainException>().Which;

        // Assert
        exception.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
        exception.Message.Should().Contain("WaitingForApproval");
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
        var (tierOfIdentityChangedDomainEvent, identityToBeDeletedDomainEvent) = activeIdentity.Should().HaveDomainEvents<TierOfIdentityChangedDomainEvent, IdentityToBeDeletedDomainEvent>();

        tierOfIdentityChangedDomainEvent.IdentityAddress.Should().Be(activeIdentity.Address);
        tierOfIdentityChangedDomainEvent.OldTierId.Should().Be(tierBeforeDeletion);
        tierOfIdentityChangedDomainEvent.NewTierId.Should().Be(Tier.QUEUED_FOR_DELETION.Id);

        identityToBeDeletedDomainEvent.IdentityAddress.Should().Be(activeIdentity.Address);
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
}
