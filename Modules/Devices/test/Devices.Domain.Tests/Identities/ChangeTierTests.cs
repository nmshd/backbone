using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class ChangeTierTests : AbstractTestsBase
{
    [Fact]
    public void Raises_TierOfIdentityChangedDomainEvent()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var oldTier = identity.TierId;

        //Act
        identity.ChangeTier(TierId.Generate());

        // Assert
        var domainEvent = identity.ShouldHaveASingleDomainEvent<TierOfIdentityChangedDomainEvent>();
        domainEvent.IdentityAddress.ShouldBe(identity.Address);
        domainEvent.OldTierId.ShouldBe(oldTier);
        domainEvent.NewTierId.ShouldBe(identity.TierId);
    }

    [Fact]
    public void Changing_the_tier_to_valid_tier_is_successful()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var newTier = TierId.Generate();

        // Act
        identity.ChangeTier(newTier);

        // Assert
        identity.TierId.ShouldBe(newTier);
    }

    [Fact]
    public void Changing_the_tier_from_QueuedForDeletion_throws_DomainException()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity(Tier.QUEUED_FOR_DELETION.Id);

        // Act
        var acting = () => identity.ChangeTier(TierId.Generate());

        // Assert
        acting.ShouldThrow<DomainException>().ShouldContainMessage(DomainErrors.CannotChangeTierQueuedForDeletion().Message);
    }

    [Fact]
    public void Changing_the_tier_to_QueuedForDeletion_throws_DomainException()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var acting = () => identity.ChangeTier(Tier.QUEUED_FOR_DELETION.Id);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldContainMessage(DomainErrors.CannotChangeTierQueuedForDeletion().Message);
    }

    [Fact]
    public void Changing_the_tier_to_the_same_tier_throws_DomainException()
    {
        // Arrange
        var tierId = TierId.Generate();
        var identity = TestDataGenerator.CreateIdentity(tierId);

        // Act
        var acting = () => identity.ChangeTier(tierId);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldContainMessage("cannot be the same");
    }
}
