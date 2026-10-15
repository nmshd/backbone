using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Tests.TestHelpers;
using FakeItEasy;
using IdentityToBeDeletedSlice = Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityToBeDeleted;

namespace Backbone.Modules.Relationships.Module.Tests.Features.DomainEvents.IdentityToBeDeleted;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task Persists_PeerToBeDeletedDomainEvent_in_outbox()
    {
        //Arrange
        var identityToBeDeleted = CreateRandomIdentityAddress();

        var peer1 = CreateRandomIdentityAddress();
        var peer2 = CreateRandomIdentityAddress();

        var relationshipToPeer1 = TestData.CreateActiveRelationship(peer1, identityToBeDeleted);
        var relationshipToPeer2 = TestData.CreateActiveRelationship(peer2, identityToBeDeleted);

        var fakeRelationshipsRepository = A.Dummy<IRelationshipsRepository>();
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

        var gracePeriodEndsAt = DateTime.Parse("2022-01-01");

        var handler = new IdentityToBeDeletedSlice.Handler(fakeRelationshipsRepository, outbox);

        //Act
        await handler.Handle(new IdentityToBeDeletedDomainEvent { IdentityAddress = identityToBeDeleted, GracePeriodEndsAt = gracePeriodEndsAt });

        //Assert
        events.Count.ShouldBe(2);
        var notifications = events.Cast<PeerToBeDeletedDomainEvent>().ToList();
        notifications.ShouldContain(e => e.PeerOfIdentityToBeDeleted == peer1 && e.RelationshipId == relationshipToPeer1.Id && e.IdentityToBeDeleted == identityToBeDeleted && e.GracePeriodEndsAt == gracePeriodEndsAt);

        notifications.ShouldContain(e => e.PeerOfIdentityToBeDeleted == peer2 && e.RelationshipId == relationshipToPeer2.Id && e.IdentityToBeDeleted == identityToBeDeleted && e.GracePeriodEndsAt == gracePeriodEndsAt);
    }

}
