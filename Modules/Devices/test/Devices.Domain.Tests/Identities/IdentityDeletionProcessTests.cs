using Backbone.Tooling;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Xunit;
using FluentAssertions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class IdentityDeletionProcessTests
{
    [Theory]
    [InlineData("2020-01-01T00:00:00")]
    [InlineData("2020-01-01T00:00:01")]
    [InlineData("2020-01-05T00:00:00")]
    public void HasApprovalPeriodExpired_is_false_for_stale_deletion_process(string time)
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01T00:00:00"));
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(DateTime.Parse(time));

        IdentityDeletionConfiguration.MaxApprovalTime = 10;

        // Act

        // Assert
        identity.DeletionProcesses[0].HasApprovalPeriodExpired.Should().BeFalse();
    }

    [Theory]
    [InlineData("2020-01-11T00:00:00")]
    [InlineData("2020-01-11T00:00:01")]
    [InlineData("2020-01-21T08:45:15")]
    [InlineData("2025-01-01T00:00:00")]
    public void HasApprovalPeriodExpired_is_true_for_deletion_process_still_in_approval_period(string time)
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01T00:00:00"));
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(DateTime.Parse(time));

        IdentityDeletionConfiguration.MaxApprovalTime = 10;

        // Act

        // Assert
        identity.DeletionProcesses[0].HasApprovalPeriodExpired.Should().BeTrue();
    }
}
