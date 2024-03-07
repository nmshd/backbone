using Backbone.Tooling;
using Xunit;
using FluentAssertions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class IdentityDeletionProcessTests
{
    [Fact]
    public void HasApprovalPeriodExpired_is_set_to_false_for_stale_deletion_process()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act

        // Assert
        identity.DeletionProcesses[0].HasApprovalPeriodExpired.Should().BeFalse();
    }

    [Fact]
    public void HasApprovalPeriodExpired_is_set_to_ture_for_deletion_process_still_in_approval_period()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();
        SystemTime.Set(SystemTime.UtcNow.AddDays(11));

        // Act
        
        // Assert
        identity.DeletionProcesses[0].HasApprovalPeriodExpired.Should().BeTrue();
    }
}
