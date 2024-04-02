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
        var identity = CreateIdentityWithApprovedDeletionProcess();

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.LengthOfGracePeriod).AddDays(1)); // past deletion grace period

        // Act
        identity.DeletionStarted();

        // Assert
        identity.Status.Should().Be(IdentityStatus.Deleting);
        identity.DeletionProcesses.First().DeletionStartedAt.Should().Be(SystemTime.UtcNow);
    }

    [Fact]
    public void Fails_to_start_if_GracePeriod_is_not_over()
    {
        // Arrange
        var identity = CreateIdentityWithApprovedDeletionProcess();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.gracePeriodHasNotYetExpired");
    }

    [Fact]
    public void Fails_to_start_if_no_deletion_process_exists()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void Fails_to_start_if_no_approved_deletion_process_exists()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        identity.StartDeletionProcessAsSupport();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    private static Identity CreateIdentityWithApprovedDeletionProcess()
    {
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        identity.StartDeletionProcessAsOwner(identity.Devices.First().Id);
        return identity;
    }
}
