using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityToBeDeleted;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Tests.TestHelpers;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.DomainEvents.Incoming;
public class IdentityToBeDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task Publishes_PeerToBeDeletedDomainEvent()
    {
        //Arrange
        var identity1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identity2 = TestDataGenerator.CreateRandomIdentityAddress();

        var identityToBeDeleted = TestDataGenerator.CreateRandomIdentityAddress();

        var relationship1 = TestData.CreateActiveRelationship(identity1, identityToBeDeleted);
        var relationship2 = TestData.CreateActiveRelationship(identity2, identityToBeDeleted);

        var fakeRelationshipsRepository = A.Dummy<IRelationshipsRepository>();

        A.CallTo(() => fakeRelationshipsRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._))
            .Returns(new List<Relationship>() { relationship1, relationship2 });

        var handler = CreateHandler(fakeRelationshipsRepository);

        //Act
        await handler.Handle(new IdentityToBeDeletedDomainEvent(identityToBeDeleted));

        //Assert
        var domainEvent1 = relationship1.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        domainEvent1.PeerOfIdentityToBeDeleted.Should().Be(identity1);
        domainEvent1.RelationshipId.Should().Be(relationship1.Id);
        domainEvent1.IdentityToBeDeleted.Should().Be(identityToBeDeleted);

        var domainEvent2 = relationship2.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        domainEvent2.PeerOfIdentityToBeDeleted.Should().Be(identity2);
        domainEvent2.RelationshipId.Should().Be(relationship2.Id);
        domainEvent2.IdentityToBeDeleted.Should().Be(identityToBeDeleted);
    }

    private static IdentityToBeDeletedDomainEventHandler CreateHandler(IRelationshipsRepository relationshipsRepository)
    {
        return new IdentityToBeDeletedDomainEventHandler(relationshipsRepository);
    }
}
