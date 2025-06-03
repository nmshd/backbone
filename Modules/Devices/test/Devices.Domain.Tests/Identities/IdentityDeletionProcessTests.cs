using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class IdentityDeletionProcessTests : AbstractTestsBase
{
    [Theory]
    [InlineData("2020-01-01T00:00:00")]
    [InlineData("2020-01-01T00:00:01")]
    [InlineData("2020-01-06T23:59:59")]
    public void HasApprovalPeriodExpired_is_false_for_deletion_process_still_in_approval_period(string utcNow)
    {
        // Arrange
        SystemTime.Set("2020-01-01T00:00:00");
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(utcNow);

        // Act
        var result = identity.DeletionProcesses[0].HasApprovalPeriodExpired;

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("2020-01-08T00:00:00")]
    [InlineData("2020-02-07T08:45:15")]
    [InlineData("2025-01-07T00:00:00")]
    public void HasApprovalPeriodExpired_is_true_for_stale_deletion_process(string utcNow)
    {
        // Arrange
        SystemTime.Set("2020-01-01T00:00:00");
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(utcNow);

        // Act
        var result = identity.DeletionProcesses[0].HasApprovalPeriodExpired;

        // Assert
        result.ShouldBeTrue();
    }
}
