using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipDeletionOfParticipantCancelledTests : AbstractTestsBase
{
    [Fact]
    public void Raises_PeerDeletionCancelledDomainEvent()
    {
        // Arrange
        var identityToBeDeleted = CreateRandomIdentityAddress();
        var peer = CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(peer, identityToBeDeleted);

        // Act
        relationship.DeletionOfParticipantCancelled(identityToBeDeleted);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<PeerDeletionCancelledDomainEvent>();
        domainEvent.PeerOfIdentityWithDeletionCancelled.Should().Be(peer);
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.IdentityWithDeletionCancelled.Should().Be(identityToBeDeleted);
    }
}
