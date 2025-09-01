using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class CancelStaleDeletionProcessTests : AbstractTestsBase
{
    [Fact]
    public void Returns_failure_if_no_process_is_waiting_for_approval()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        // Act
        var result = identity.CancelStaleDeletionProcess();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void Returns_failure_if_process_is_still_within_approval_period()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        // Act
        var result = identity.CancelStaleDeletionProcess();

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Active);
        deletionProcess.Status.ShouldBe(DeletionProcessStatus.WaitingForApproval);

        result.IsFailure.ShouldBeTrue();
        result.Error.Message.ShouldBe("No deletion process is past due approval.");
        result.Error.Code.ShouldBe("error.platform.validation.device.noDeletionProcessIsPastDueApproval");
    }

    [Fact]
    public void Cancel_deletion_process_that_is_past_due_approval()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01T00:00:00"));
        var identity = TestDataGenerator.CreateIdentity();
        identity.StartDeletionProcessAsSupport();

        var utcNow = DateTime.Parse("2020-01-08T00:00:00");
        SystemTime.Set(utcNow);

        // Act
        var result = identity.CancelStaleDeletionProcess();

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Active);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Status.ShouldBe(DeletionProcessStatus.Cancelled);
        result.Value.CancelledAt.ShouldBe(utcNow);
    }

    [Fact]
    public void Creates_audit_log_entry_when_executed()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01T00:00:00"));
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(DateTime.Parse("2020-01-08T00:00:00"));

        // Act
        var result = identity.CancelStaleDeletionProcess();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AuditLog.ShouldHaveCount(2); // count 2 because the first one was creation of the deletion process
        result.Value.AuditLog[1].OldStatus.ShouldBe(DeletionProcessStatus.WaitingForApproval);
        result.Value.AuditLog[1].NewStatus.ShouldBe(DeletionProcessStatus.Cancelled);
    }

    [Fact]
    public void Raises_IdentityDeletionProcessStatusChangedDomainEvent()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01T00:00:00"));
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(DateTime.Parse("2020-01-08T00:00:00"));
        identity.DeletionProcesses[0].ClearDomainEvents();

        // Act
        var deletionProcessResult = identity.CancelStaleDeletionProcess();

        deletionProcessResult.IsSuccess.ShouldBeTrue();

        var deletionProcess = deletionProcessResult.Value;

        var domainEvent = deletionProcess.ShouldHaveASingleDomainEvent<IdentityDeletionProcessStatusChangedDomainEvent>();
        domainEvent.DeletionProcessId.ShouldBe(deletionProcess.Id);
        domainEvent.DeletionProcessOwner.ShouldBe(identity.Address);
        domainEvent.Initiator.ShouldBe(null);
    }
}
