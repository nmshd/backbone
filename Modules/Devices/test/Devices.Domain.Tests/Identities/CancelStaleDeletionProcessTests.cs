using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;

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
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
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
        identity.Status.Should().Be(IdentityStatus.Active);
        deletionProcess.Status.Should().Be(DeletionProcessStatus.WaitingForApproval);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("No deletion process is past due approval.");
        result.Error.Code.Should().Be("error.platform.validation.device.noDeletionProcessIsPastDueApproval");
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
        identity.Status.Should().Be(IdentityStatus.Active);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(DeletionProcessStatus.Cancelled);
        result.Value.CancelledAt.Should().Be(utcNow);
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
        result.IsSuccess.Should().BeTrue();
        result.Value.AuditLog.Should().HaveCount(2); // count 2 because the first one was creation of the deletion process
        result.Value.AuditLog[1].ProcessId.Should().Be(identity.DeletionProcesses[0].Id);
        result.Value.AuditLog[1].OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        result.Value.AuditLog[1].NewStatus.Should().Be(DeletionProcessStatus.Cancelled);
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

        deletionProcessResult.IsSuccess.Should().BeTrue();

        var deletionProcess = deletionProcessResult.Value;

        var domainEvent = deletionProcess.Should().HaveASingleDomainEvent<IdentityDeletionProcessStatusChangedDomainEvent>();
        domainEvent.DeletionProcessId.Should().Be(deletionProcess.Id);
        domainEvent.Address.Should().Be(identity.Address);
        domainEvent.Initiator.Should().Be(null);
    }
}
