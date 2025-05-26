using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

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
        identity.Status.ShouldBe(IdentityStatus.Deleting);
        identity.DeletionProcesses[0].DeletionStartedAt.ShouldBe(SystemTime.UtcNow);
    }

    [Fact]
    public void Fails_to_start_if_GracePeriod_is_not_over()
    {
        // Arrange
        var identity = CreateIdentityWithApprovedDeletionProcess();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.gracePeriodHasNotYetExpired");
    }

    [Fact]
    public void Fails_to_start_if_no_deletion_process_exists()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
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
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
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
        var domainEvent = activeIdentity.ShouldHaveASingleDomainEvent<IdentityDeletedDomainEvent>();
        domainEvent.IdentityAddress.ShouldBe(activeIdentity.Address);
    }

    private static Identity CreateIdentityWithApprovedDeletionProcess()
    {
        var identity = TestDataGenerator.CreateIdentity();
        identity.StartDeletionProcessAsOwner(identity.Devices.First().Id);

        return identity;
    }
}
