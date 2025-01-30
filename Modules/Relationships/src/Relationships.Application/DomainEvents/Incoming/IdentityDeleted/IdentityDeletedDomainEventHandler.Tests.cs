using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using FakeItEasy;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeleted;

public class IdentityDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task Publishes_PeerToBeDeletedDomainEvent()
    {
        //Arrange
        var identityToBeDeleted = CreateRandomIdentityAddress();

        var peer1 = CreateRandomIdentityAddress();
        var peer2 = CreateRandomIdentityAddress();

        var relationshipToPeer1 = TestData.CreateActiveRelationship(peer1, identityToBeDeleted);
        var relationshipToPeer2 = TestData.CreateActiveRelationship(peer2, identityToBeDeleted);

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();

        A.CallTo(() => fakeRelationshipsRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Relationship> { relationshipToPeer1, relationshipToPeer2 });

        var handler = CreateHandler(fakeRelationshipsRepository);

        //Act
        await handler.Handle(new IdentityDeletedDomainEvent(identityToBeDeleted));

        //Assert
        var event1 = relationshipToPeer1.Should().HaveASingleDomainEvent<PeerDeletedDomainEvent>();
        event1.PeerOfDeletedIdentity.Should().Be(peer1);
        event1.RelationshipId.Should().Be(relationshipToPeer1.Id);
        event1.DeletedIdentity.Should().Be(identityToBeDeleted);

        var event2 = relationshipToPeer2.Should().HaveASingleDomainEvent<PeerDeletedDomainEvent>();
        event2.PeerOfDeletedIdentity.Should().Be(peer2);
        event2.RelationshipId.Should().Be(relationshipToPeer2.Id);
        event2.DeletedIdentity.Should().Be(identityToBeDeleted);
    }

    private static IdentityDeletedDomainEventHandler CreateHandler(IRelationshipsRepository relationshipsRepository)
    {
        return new IdentityDeletedDomainEventHandler(relationshipsRepository, A.Dummy<IEventBus>());
    }
}
