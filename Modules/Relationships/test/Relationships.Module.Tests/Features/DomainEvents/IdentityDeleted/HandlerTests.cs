using IdentityDeletedSlice = Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityDeleted;
using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Module.Tests.TestHelpers;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FakeItEasy;

namespace Backbone.Modules.Relationships.Module.Tests.Features.DomainEvents.IdentityDeleted;

public class HandlerTests : AbstractTestsBase
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

        A.CallTo(() => fakeRelationshipsRepository.ListWithoutContent(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._, A<bool>._, A<bool>._))
            .Returns([relationshipToPeer1, relationshipToPeer2]);

        var handler = CreateHandler(fakeRelationshipsRepository, mockEventBus);

        //Act
        await handler.Handle(new IdentityDeletedDomainEvent { IdentityAddress = identityToBeDeleted });

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

    private static IdentityDeletedSlice.Handler CreateHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        return new IdentityDeletedSlice.Handler(relationshipsRepository, eventBus);
    }
}
