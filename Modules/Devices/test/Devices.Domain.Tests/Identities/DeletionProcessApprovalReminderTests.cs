using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionProcessApprovalReminderTests : AbstractTestsBase
{
    [Fact]
    public void DeletionProcessApprovalReminder1Sent_updates_ApprovalReminder1SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        identity.DeletionProcessApprovalReminder1Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.WaitingForApproval)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.ApprovalReminder1SentAt.ShouldBe(currentDateTime);
    }

    [Fact]
    public void DeletionProcessApprovalReminder1Sent_fails_when_no_deletion_process_waiting_for_approval_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionProcessApprovalReminder1Sent;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void DeletionProcessApprovalReminder2Sent_updates_ApprovalReminder2SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        identity.DeletionProcessApprovalReminder2Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.WaitingForApproval)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.ApprovalReminder2SentAt.ShouldBe(currentDateTime);
    }


    [Fact]
    public void DeletionProcessApprovalReminder2Sent_fails_when_no_deletion_process_waiting_for_approval_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionProcessApprovalReminder2Sent;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void DeletionProcessApprovalReminder3Sent_updates_ApprovalReminder3SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        identity.DeletionProcessApprovalReminder3Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.WaitingForApproval)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.ApprovalReminder3SentAt.ShouldBe(currentDateTime);
    }


    [Fact]
    public void DeletionProcessApprovalReminder3Sent_fails_when_no_deletion_process_waiting_for_approval_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionProcessApprovalReminder3Sent;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.ShouldHaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.ProcessId.ShouldBe(deletionProcess.Id);
        auditLogEntry.CreatedAt.ShouldBe(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.ShouldBeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.ShouldBe(DeletionProcessStatus.WaitingForApproval);
        auditLogEntry.NewStatus.ShouldBe(DeletionProcessStatus.WaitingForApproval);
    }

    private static Identity CreateIdentityWithDeletionProcessWaitingForApproval()
    {
        var identity = CreateIdentity();
        Hasher.SetHasher(new DummyHasher([1, 2, 3]));
        identity.StartDeletionProcessAsSupport();

        return identity;
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create([], "prod.enmeshed.eu");
        return new Identity("", address, [], TierId.Generate(), 1, CommunicationLanguage.DEFAULT_LANGUAGE);
    }

    public override void Dispose()
    {
        base.Dispose();
        Hasher.Reset();
    }
}
