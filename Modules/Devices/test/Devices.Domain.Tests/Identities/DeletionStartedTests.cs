using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionStartedTests
{
    [Fact]
    public void DeletionStarted_sets_status_and_creates_valid_DeletionProcess()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01 06:00:00");
        SystemTime.Set(currentDateTime);
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        identity.StartDeletionProcessAsOwner(identity.Devices.First().Id);

        // Act
        identity.DeletionStarted();

        // Assert
        identity.Status.Should().Be(IdentityStatus.Deleting);
        identity.DeletionProcesses.Should().HaveCount(1);
        identity.DeletionProcesses.First().DeletionStartedAt.Should().Be(currentDateTime);

    }
}
