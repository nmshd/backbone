using Backbone.BuildingBlocks.Domain;
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
        SystemTime.Set(DateTime.Parse("2000-01-01 06:00:00"));
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        identity.StartDeletionProcessAsOwner(identity.Devices.First().Id);

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.LengthOfGracePeriod).AddDays(1)); // past deletion grace period
        

        // Act
        identity.DeletionStarted();

        // Assert
        identity.Status.Should().Be(IdentityStatus.Deleting);
        identity.DeletionProcesses.Should().HaveCount(1);
        identity.DeletionProcesses.First().DeletionStartedAt.Should().Be(SystemTime.UtcNow);
    }

    [Fact]
    public void Fails_to_start_if_GracePeriod_is_not_over()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01 06:00:00");
        SystemTime.Set(currentDateTime);
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        identity.StartDeletionProcessAsOwner(identity.Devices.First().Id);

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be(DomainErrors.IdentityCannotBeDeleted().Code);
    }
}
