using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

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
        var domainEvent = identity.Should().HaveASingleDomainEvent<TierOfIdentityChangedDomainEvent>();
        domainEvent.IdentityAddress.Should().Be(identity.Address);
        domainEvent.OldTierId.Should().Be(oldTier);
        domainEvent.NewTierId.Should().Be(identity.TierId);
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
        identity.TierId.Should().Be(newTier);
    }

    [Fact]
    public void Changing_the_tier_from_QueuedForDeletion_throws_DomainException()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity(Tier.QUEUED_FOR_DELETION.Id);

        // Act
        var acting = () => identity.ChangeTier(TierId.Generate());

        // Assert
        acting.Should().Throw<DomainException>().Which.Message.Should().Be(DomainErrors.CannotChangeTierQueuedForDeletion().Message);
    }

    [Fact]
    public void Changing_the_tier_to_QueuedForDeletion_throws_DomainException()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var acting = () => identity.ChangeTier(Tier.QUEUED_FOR_DELETION.Id);

        // Assert
        acting.Should().Throw<DomainException>().Which.Message.Should().Be(DomainErrors.CannotChangeTierQueuedForDeletion().Message);
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
        acting.Should().Throw<DomainException>().Which.Message.Should().Contain("cannot be the same");
    }
}
