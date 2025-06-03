using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class CancelDeletionProcessAsSupportTests : AbstractTestsBase
{
    [Fact]
    public void Cancel_deletion_process()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2024-01-01"));
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var tierIdBeforeDeletion = identity.TierIdBeforeDeletion;

        // Act
        var deletionProcess = identity.CancelDeletionProcessAsSupport(identity.DeletionProcesses[0].Id);

        // Assert
        identity.TierId.ShouldBe(tierIdBeforeDeletion);
        identity.TierIdBeforeDeletion.ShouldBe(null);
        identity.Status.ShouldBe(IdentityStatus.Active);
        deletionProcess.Status.ShouldBe(DeletionProcessStatus.Cancelled);
        deletionProcess.CancelledAt.ShouldBe(DateTime.Parse("2024-01-01"));
        deletionProcess.CancelledByDevice.ShouldBe(null!);
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
        var deletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;

        // Act
        var acting = () => identity.CancelDeletionProcessAsSupport(deletionProcessId);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldContainMessage("IdentityDeletionProcess");
    }

    [Fact]
    public void Throws_when_deletion_process_is_in_wrong_status()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        var acting = () => identity.CancelDeletionProcessAsSupport(identity.DeletionProcesses[0].Id);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void Raises_domain_events()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        // Act
        var deletionProcess = identity.CancelDeletionProcessAsSupport(identity.DeletionProcesses[0].Id);

        // Assert
        var deletionProcessDomainEvent = deletionProcess.ShouldHaveASingleDomainEvent<IdentityDeletionProcessStatusChangedDomainEvent>();
        deletionProcessDomainEvent.DeletionProcessId.ShouldBe(deletionProcess.Id);
        deletionProcessDomainEvent.Address.ShouldBe(identity.Address);
        deletionProcessDomainEvent.Initiator.ShouldBe(null);

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
        auditLogEntry.ProcessId.ShouldBe(deletionProcess.Id);
        auditLogEntry.OldStatus.ShouldBe(DeletionProcessStatus.Approved);
        auditLogEntry.NewStatus.ShouldBe(DeletionProcessStatus.Cancelled);
    }
}
