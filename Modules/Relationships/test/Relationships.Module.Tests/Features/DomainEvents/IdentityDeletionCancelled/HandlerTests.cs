using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Tests.TestHelpers;
using FakeItEasy;
using IdentityDeletionCancelledSlice = Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityDeletionCancelled;

namespace Backbone.Modules.Relationships.Module.Tests.Features.DomainEvents.IdentityDeletionCancelled;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task Persists_PeerDeletionCancelledDomainEvent_in_outbox()
    {
        //Arrange
        var identityWithDeletionCancelled = CreateRandomIdentityAddress();

        var peer1 = CreateRandomIdentityAddress();
        var peer2 = CreateRandomIdentityAddress();

        var relationshipToPeer1 = TestData.CreateActiveRelationship(peer1, identityWithDeletionCancelled);
        var relationshipToPeer2 = TestData.CreateActiveRelationship(peer2, identityWithDeletionCancelled);

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var outbox = A.Fake<IEventOutbox<RelationshipsModule>>();
        var events = new List<DomainEvent>();
        A.CallTo(() => outbox.Enqueue(A<IEnumerable<(DomainEvent Event, string Stream)>>._, A<CancellationToken>._))
            .ReturnsLazily((IEnumerable<(DomainEvent Event, string Stream)> notifications, CancellationToken _) =>
            {
                events.AddRange(notifications.Select(x => x.Event));
                return Task.CompletedTask;
            });

        A.CallTo(() => fakeRelationshipsRepository.ListWithoutContent(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._, A<bool>._, A<bool>._))
            .Returns([relationshipToPeer1, relationshipToPeer2]);

        var handler = new IdentityDeletionCancelledSlice.Handler(fakeRelationshipsRepository, outbox);

        //Act
        await handler.Handle(new IdentityDeletionCancelledDomainEvent { IdentityAddress = identityWithDeletionCancelled });

        //Assert
        events.Count.ShouldBe(2);
        var notifications = events.Cast<PeerDeletionCancelledDomainEvent>().ToList();
        notifications.ShouldContain(e => e.PeerOfIdentityWithDeletionCancelled == peer1 && e.RelationshipId == relationshipToPeer1.Id && e.IdentityWithDeletionCancelled == identityWithDeletionCancelled);

        notifications.ShouldContain(e => e.PeerOfIdentityWithDeletionCancelled == peer2 && e.RelationshipId == relationshipToPeer2.Id && e.IdentityWithDeletionCancelled == identityWithDeletionCancelled);
    }

}
