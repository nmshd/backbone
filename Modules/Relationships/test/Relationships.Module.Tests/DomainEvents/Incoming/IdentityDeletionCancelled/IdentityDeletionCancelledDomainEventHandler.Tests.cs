using IdentityDeletionCancelledSlice = Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityDeletionCancelled;
using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Module.Tests.TestHelpers;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FakeItEasy;

namespace Backbone.Modules.Relationships.Module.Tests.DomainEvents.Incoming.IdentityDeletionCancelled;

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

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var mockEventBus = A.Fake<IEventBus>();

        A.CallTo(() => fakeRelationshipsRepository.ListWithoutContent(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._, A<bool>._, A<bool>._))
            .Returns([relationshipToPeer1, relationshipToPeer2]);

        var handler = CreateHandler(fakeRelationshipsRepository, mockEventBus);

        //Act
        await handler.Handle(new IdentityDeletionCancelledDomainEvent { IdentityAddress = identityWithDeletionCancelled });

        //Assert
        A.CallTo(() => mockEventBus.Publish(A<PeerDeletionCancelledDomainEvent>.That.Matches(e => e.PeerOfIdentityWithDeletionCancelled == peer1 &&
                                                                                                  e.RelationshipId == relationshipToPeer1.Id &&
                                                                                                  e.IdentityWithDeletionCancelled == identityWithDeletionCancelled)))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(A<PeerDeletionCancelledDomainEvent>.That.Matches(e => e.PeerOfIdentityWithDeletionCancelled == peer2 &&
                                                                                                  e.RelationshipId == relationshipToPeer2.Id &&
                                                                                                  e.IdentityWithDeletionCancelled == identityWithDeletionCancelled)))
            .MustHaveHappenedOnceExactly();
    }

    private static IdentityDeletionCancelledSlice.Handler CreateHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        return new IdentityDeletionCancelledSlice.Handler(relationshipsRepository, eventBus);
    }
}
