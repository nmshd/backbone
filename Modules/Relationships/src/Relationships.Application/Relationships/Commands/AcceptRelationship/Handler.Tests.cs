using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationship;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Returns_the_updated_relationship()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreatePendingRelationship(to: activeIdentity);
        A.CallTo(() => fakeRelationshipsRepository.FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var handler = CreateHandler(fakeUserContext, fakeRelationshipsRepository);

        // Act
        var response = await handler.Handle(new AcceptRelationshipCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        response.Id.Should().NotBeNull();
        response.Status.Should().Be(RelationshipStatus.Active);
        response.AuditLog.Should().HaveCount(2);
    }

    [Fact]
    public async Task Saves_the_updated_relationship()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var mockRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreatePendingRelationship(to: activeIdentity);
        A.CallTo(() => mockRelationshipsRepository.FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var handler = CreateHandler(fakeUserContext, mockRelationshipsRepository);

        // Act
        await handler.Handle(new AcceptRelationshipCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        A.CallTo(
                () => mockRelationshipsRepository.Update(
                    A<Relationship>.That.Matches(r => r.Id == relationship.Id && r.Status == RelationshipStatus.Active))
            )
            .MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IUserContext userContext, IRelationshipsRepository relationshipsRepository, IEventBus? eventBus = null)
    {
        eventBus ??= A.Fake<IEventBus>();
        return new Handler(relationshipsRepository, userContext, eventBus);
    }
}
