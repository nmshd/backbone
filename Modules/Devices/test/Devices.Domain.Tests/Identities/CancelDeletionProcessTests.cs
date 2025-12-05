using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class CancelDeletionProcessTests : AbstractTestsBase
{
    [Fact]
    public void Cancel_deletion_process()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2024-01-01"));
        var identity = TestDataGenerator.CreateIdentityWithActiveDeletionProcess();
        var tierIdBeforeDeletion = identity.TierIdBeforeDeletion;

        // Act
        var deletionProcess = identity.CancelDeletionProcess(identity.DeletionProcesses[0].Id, identity.Devices[0].Id);

        // Assert
        identity.TierId.ShouldBe(tierIdBeforeDeletion);
        identity.TierIdBeforeDeletion.ShouldBe(null);
        identity.Status.ShouldBe(IdentityStatus.Active);
        identity.DeletionGracePeriodEndsAt.ShouldBeNull();
        deletionProcess.Status.ShouldBe(DeletionProcessStatus.Cancelled);
        deletionProcess.GracePeriodEndsAt.ShouldBeNull();
        deletionProcess.CancelledAt.ShouldBe(DateTime.Parse("2024-01-01"));
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
        var deviceId = identity.Devices[0].Id;
        var deletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;

        // Act
        var acting = () => identity.CancelDeletionProcess(deletionProcessId, deviceId);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldContainMessage("IdentityDeletionProcess");
    }

    [Fact]
    public void Throws_when_grace_period_has_expired()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithActiveDeletionProcess();

        SystemTime.Set(DateTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays));

        // Act
        var acting = () => identity.CancelDeletionProcess(identity.DeletionProcesses[0].Id, identity.Devices[0].Id);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.gracePeriodHasAlreadyExpired");
    }

    [Fact]
    public void Raises_domain_events()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithActiveDeletionProcess();
        identity.DeletionProcesses[0].ClearDomainEvents();

        // Act
        var deletionProcess = identity.CancelDeletionProcess(identity.DeletionProcesses[0].Id, identity.Devices[0].Id);

        // Assert
        var deletionProcessDomainEvent = deletionProcess.ShouldHaveASingleDomainEvent<IdentityDeletionProcessStatusChangedDomainEvent>();
        deletionProcessDomainEvent.DeletionProcessId.ShouldBe(deletionProcess.Id);
        deletionProcessDomainEvent.DeletionProcessOwner.ShouldBe(identity.Address);
        deletionProcessDomainEvent.Initiator.ShouldBe(identity.Address);

        var (tierOfIdentityChangedDomainEvent, identityDeletionCancelledDomainEvent) = identity.ShouldHaveDomainEvents<TierOfIdentityChangedDomainEvent, IdentityDeletionCancelledDomainEvent>();

        tierOfIdentityChangedDomainEvent.IdentityAddress.ShouldBe(identity.Address);
        tierOfIdentityChangedDomainEvent.OldTierId.ShouldBe(Tier.QUEUED_FOR_DELETION.Id);
        tierOfIdentityChangedDomainEvent.NewTierId.ShouldBe(identity.TierId);

        identityDeletionCancelledDomainEvent.IdentityAddress.ShouldBe(identity.Address);
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.ShouldHaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.MessageKey.ShouldBe(MessageKey.CancelledByOwner);
        auditLogEntry.OldStatus.ShouldBe(DeletionProcessStatus.Active);
        auditLogEntry.NewStatus.ShouldBe(DeletionProcessStatus.Cancelled);
    }
}
