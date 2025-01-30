using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using FakeItEasy;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeletionCancelled;

public class IdentityDeletionCancelledDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task Publishes_PeerToBeDeletedDomainEvent()
    {
        //Arrange
        var identityWithDeletionCancelled = CreateRandomIdentityAddress();

        var peer1 = CreateRandomIdentityAddress();
        var peer2 = CreateRandomIdentityAddress();

        var relationshipToPeer1 = TestData.CreateActiveRelationship(peer1, identityWithDeletionCancelled);
        var relationshipToPeer2 = TestData.CreateActiveRelationship(peer2, identityWithDeletionCancelled);

        var fakeRelationshipsRepository = A.Dummy<IRelationshipsRepository>();

        A.CallTo(() => fakeRelationshipsRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Relationship> { relationshipToPeer1, relationshipToPeer2 });

        var handler = CreateHandler(fakeRelationshipsRepository);

        //Act
        await handler.Handle(new IdentityDeletionCancelledDomainEvent(identityWithDeletionCancelled));

        //Assert
        var event1 = relationshipToPeer1.Should().HaveASingleDomainEvent<PeerDeletionCancelledDomainEvent>();
        event1.PeerOfIdentityWithDeletionCancelled.Should().Be(peer1);
        event1.RelationshipId.Should().Be(relationshipToPeer1.Id);
        event1.IdentityWithDeletionCancelled.Should().Be(identityWithDeletionCancelled);

        var event2 = relationshipToPeer2.Should().HaveASingleDomainEvent<PeerDeletionCancelledDomainEvent>();
        event2.PeerOfIdentityWithDeletionCancelled.Should().Be(peer2);
        event2.RelationshipId.Should().Be(relationshipToPeer2.Id);
        event2.IdentityWithDeletionCancelled.Should().Be(identityWithDeletionCancelled);
    }

    private static IdentityDeletionCancelledDomainEventHandler CreateHandler(IRelationshipsRepository relationshipsRepository)
    {
        return new IdentityDeletionCancelledDomainEventHandler(relationshipsRepository, A.Dummy<IEventBus>());
    }
}
