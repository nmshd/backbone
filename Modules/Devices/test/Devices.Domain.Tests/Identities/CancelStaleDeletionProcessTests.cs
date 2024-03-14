﻿using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class CancelStaleDeletionProcessTests
{
    [Fact]
    public void Returns_failure_if_no_process_is_waiting_for_approval()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        // Act
        var result = identity.CancelStaleDeletionProcess();

        // Assert
        result.Error.Message.Should().Be($"The deletion process must be in status 'WaitingForApproval'.");
        result.Error.Code.Should().Be($"error.platform.validation.device.deletionProcessMustBeInStatusWaitingForApproval");
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

        result.Error.Message.Should().Be("No deletion process is past due approval.");
        result.Error.Code.Should().Be("error.platform.validation.device.noDeletionProcessIsPastDueApproval");
    }

    [Fact]
    public void Cancel_deletion_process_that_is_past_due_approval()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01T00:00:00"));
        var identity = TestDataGenerator.CreateIdentity();
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        var utcNow = DateTime.Parse("2020-01-11T00:00:00");
        SystemTime.Set(utcNow);

        IdentityDeletionConfiguration.MaxApprovalTime = 10;

        // Act
        var result = identity.CancelStaleDeletionProcess();

        // Assert
        identity.Status.Should().Be(IdentityStatus.Active);

        deletionProcess.Status.Should().Be(DeletionProcessStatus.Cancelled);
        deletionProcess.CancelledAt.Should().Be(utcNow);

        result.Should().Be(Result.Success<IdentityDeletionProcess, DomainError>(deletionProcess));
    }

    [Fact]
    public void Canceling_stale_deletion_process_creates_audit_log_entry_when_executed()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01T00:00:00"));
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(DateTime.Parse("2020-01-11T00:00:00"));

        IdentityDeletionConfiguration.MaxApprovalTime = 10;

        // Act
        var deletionProcess = identity.CancelStaleDeletionProcess();

        // Assert
        deletionProcess.Value.AuditLog.Should().HaveCount(2); // count 2 because the first one was creation of the deletion process
        deletionProcess.Value.AuditLog[1].ProcessId.Should().Be(identity.DeletionProcesses[0].Id);
        deletionProcess.Value.AuditLog[1].OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        deletionProcess.Value.AuditLog[1].NewStatus.Should().Be(DeletionProcessStatus.Cancelled);
    }
}
