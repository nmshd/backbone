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
        var mockEventBus = A.Fake<IEventBus>();

        A.CallTo(() => fakeRelationshipsRepository.List(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._, A<bool>._))
            .Returns([relationshipToPeer1, relationshipToPeer2]);

        var handler = CreateHandler(fakeRelationshipsRepository, mockEventBus);

        //Act
        await handler.Handle(new IdentityDeletedDomainEvent(identityToBeDeleted));

        //Assert
        A.CallTo(() => mockEventBus.Publish(A<PeerDeletedDomainEvent>.That.Matches(e => e.PeerOfDeletedIdentity == peer1 &&
                                                                                        e.RelationshipId == relationshipToPeer1.Id &&
                                                                                        e.DeletedIdentity == identityToBeDeleted)))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(A<PeerDeletedDomainEvent>.That.Matches(e => e.PeerOfDeletedIdentity == peer2 &&
                                                                                        e.RelationshipId == relationshipToPeer2.Id &&
                                                                                        e.DeletedIdentity == identityToBeDeleted)))
            .MustHaveHappenedOnceExactly();
    }

    private static IdentityDeletedDomainEventHandler CreateHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        return new IdentityDeletedDomainEventHandler(relationshipsRepository, eventBus);
    }
}
