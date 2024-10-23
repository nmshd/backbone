using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipDeletionOfParticipantStartedTests : AbstractTestsBase
{
    [Fact]
    public void Raises_PeerDeletedDomainEvent()
    {
        // Arrange
        var identityToBeDeleted = CreateRandomIdentityAddress();
        var peer = CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(peer, identityToBeDeleted);

        // Act
        relationship.DeletionOfParticipantStarted(identityToBeDeleted);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<PeerDeletedDomainEvent>();
        domainEvent.PeerOfDeletedIdentity.Should().Be(peer);
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.DeletedIdentity.Should().Be(identityToBeDeleted);
    }
}
