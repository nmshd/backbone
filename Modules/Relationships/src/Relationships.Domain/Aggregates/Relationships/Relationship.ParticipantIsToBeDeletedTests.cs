using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipParticipantIsToBeDeletedTests : AbstractTestsBase
{
    [Fact]
    public void Raises_PeerToBeDeletedDomainEvent()
    {
        // Arrange
        var identityToBeDeleted = CreateRandomIdentityAddress();
        var peer = CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(peer, identityToBeDeleted);
        var gracePeriodEndsAt = DateTime.Parse("2022-01-01");

        // Act
        relationship.ParticipantIsToBeDeleted(identityToBeDeleted, gracePeriodEndsAt);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        domainEvent.PeerOfIdentityToBeDeleted.Should().Be(peer);
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.IdentityToBeDeleted.Should().Be(identityToBeDeleted);
        domainEvent.GracePeriodEndsAt.Should().Be(gracePeriodEndsAt);
    }
}
