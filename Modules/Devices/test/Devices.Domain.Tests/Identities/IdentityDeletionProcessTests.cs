using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class IdentityDeletionProcessTests
{
    [Theory]
    [InlineData("2020-01-01T00:00:00")]
    [InlineData("2020-01-01T00:00:01")]
    [InlineData("2020-01-09T23:59:59")]
    public void HasApprovalPeriodExpired_is_false_for_deletion_process_still_in_approval_period(string utcNow)
    {
        // Arrange
        SystemTime.Set("2020-01-01T00:00:00");
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(utcNow);

        IdentityDeletionConfiguration.MaxApprovalTime = 10;

        // Act
        var result = identity.DeletionProcesses[0].HasApprovalPeriodExpired;

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("2020-01-11T00:00:00")]
    [InlineData("2020-01-21T08:45:15")]
    [InlineData("2025-01-01T00:00:00")]
    public void HasApprovalPeriodExpired_is_true_for_stale_deletion_process(string utcNow)
    {
        // Arrange
        SystemTime.Set("2020-01-01T00:00:00");
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(utcNow);

        IdentityDeletionConfiguration.MaxApprovalTime = 10;

        // Act
        var result = identity.DeletionProcesses[0].HasApprovalPeriodExpired;

        // Assert
        result.Should().BeTrue();
    }
}
