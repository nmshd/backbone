using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.CanEstablishRelationship;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Two_valid_identities_without_relationship_returns_true()
    {
        // Arrange
        var identity = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        var fakeContext = CreateUserContext(identity);
        var fakeRepository = CreateRepository([]);
        var handler = new Handler(fakeContext, fakeRepository);

        // Act
        var response = await handler.Handle(new CanEstablishRelationshipQuery { PeerAddress = peer }, CancellationToken.None);

        // Assert
        response.CanCreate.Should().BeTrue();
    }

    [Fact]
    public async Task Two_valid_identities_with_active_relationship_returns_false()
    {
        // Arrange
        var identity = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        var activeRelationship = TestData.CreateActiveRelationship(identity, peer);
        var handler = new Handler(CreateUserContext(identity), CreateRepository([activeRelationship]));

        // Act
        var response = await handler.Handle(new CanEstablishRelationshipQuery { PeerAddress = peer }, CancellationToken.None);

        // Assert
        response.CanCreate.Should().BeFalse();
    }

    [Fact]
    public async Task Two_valid_identities_with_rejected_relationship_returns_true()
    {
        // Arrange
        var identity = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        var rejectedRelationship = TestData.CreatePendingRelationship(identity, peer);
        rejectedRelationship.Reject(peer, TestDataGenerator.CreateRandomDeviceId(), null);
        var handler = new Handler(CreateUserContext(identity), CreateRepository([rejectedRelationship]));

        // Act
        var response = await handler.Handle(new CanEstablishRelationshipQuery { PeerAddress = peer }, CancellationToken.None);

        // Assert
        response.CanCreate.Should().BeTrue();
    }

    [Fact]
    public async Task Two_valid_identities_with_rejected_and_active_relationship_returns_false()
    {
        // Arrange
        var identity = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        var rejectedRelationship = TestData.CreatePendingRelationship(identity, peer);
        rejectedRelationship.Reject(peer, TestDataGenerator.CreateRandomDeviceId(), null);
        var activeRelationship = TestData.CreateActiveRelationship(identity, peer);
        var handler = new Handler(CreateUserContext(identity), CreateRepository([activeRelationship, rejectedRelationship]));

        // Act
        var response = await handler.Handle(new CanEstablishRelationshipQuery { PeerAddress = peer }, CancellationToken.None);

        // Assert
        response.CanCreate.Should().BeFalse();
    }

    [Fact]
    public async Task Two_valid_identities_with_multiple_rejected_relationships_returns_true()
    {
        // Arrange
        var identity = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        List<Relationship> relationships = [];

        for (var i = 0; i < 3; i++)
        {
            var rejectedRelationship = TestData.CreatePendingRelationship(identity, peer);
            rejectedRelationship.Reject(peer, TestDataGenerator.CreateRandomDeviceId(), null);
            relationships.Add(rejectedRelationship);
        }

        var handler = new Handler(CreateUserContext(identity), CreateRepository(relationships));

        // Act
        var response = await handler.Handle(new CanEstablishRelationshipQuery { PeerAddress = peer }, CancellationToken.None);

        // Assert
        response.CanCreate.Should().BeTrue();
    }

    private static IUserContext CreateUserContext(IdentityAddress address)
    {
        var fakeContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeContext.GetAddress()).Returns(address);
        return fakeContext;
    }

    private static IRelationshipsRepository CreateRepository(IEnumerable<Relationship> relationships)
    {
        var fakeRepository = A.Fake<IRelationshipsRepository>();
        A.CallTo(() => fakeRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._)).Returns(relationships);
        return fakeRepository;
    }
}
