using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionStartedTests : AbstractTestsBase
{
    [Fact]
    public void DeletionStarted_sets_status_and_creates_valid_DeletionProcess()
    {
        // Arrange
        var identity = CreateIdentityWithApprovedDeletionProcess();

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays).AddDays(1)); // past deletion grace period

        // Act
        identity.DeletionStarted();

        // Assert
        identity.Status.Should().Be(IdentityStatus.Deleting);
        identity.DeletionProcesses[0].DeletionStartedAt.Should().Be(SystemTime.UtcNow);
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
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void Fails_to_start_if_no_approved_deletion_process_exists()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        identity.StartDeletionProcessAsSupport();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void Raises_domain_events()
    {
        //Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays).AddDays(1));

        //Act
        activeIdentity.DeletionStarted();

        //Assert
        var domainEvent = activeIdentity.Should().HaveASingleDomainEvent<IdentityDeletedDomainEvent>();
        domainEvent.IdentityAddress.Should().Be(activeIdentity.Address);
    }

    private static Identity CreateIdentityWithApprovedDeletionProcess()
    {
        var identity = TestDataGenerator.CreateIdentity();
        identity.StartDeletionProcessAsOwner(identity.Devices.First().Id);

        return identity;
    }
}
