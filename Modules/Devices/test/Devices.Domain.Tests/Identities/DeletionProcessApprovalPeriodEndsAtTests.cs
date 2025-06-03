using Backbone.Tooling;
using static Backbone.Modules.Devices.Domain.Tests.TestDataGenerator;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionProcessApprovalPeriodEndsAtTests : AbstractTestsBase
{
    [Fact]
    public void Deletion_process_ApprovalPeriodEndsAt_returns_expected_date()
    {
        // Arrange
        SystemTime.Set("2024-01-01");

        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();
        var deletionProcess = identity.DeletionProcesses[0];

        // Act
        var approvalPeriodEndsAt = deletionProcess.ApprovalPeriodEndsAt;

        // Assert
        approvalPeriodEndsAt.ShouldBe(DateTime.Parse("2024-01-08"));
    }
}
