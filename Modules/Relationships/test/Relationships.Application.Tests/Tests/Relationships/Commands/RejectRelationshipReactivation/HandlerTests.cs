using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipReactivation;
using Backbone.Modules.Relationships.Application.Tests.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.RejectRelationshipReactivation;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var identityTo = TestDataGenerator.CreateRandomIdentityAddress();

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreateRelationshipWithRequestedReactivation(from: activeIdentity, to: identityTo, reactivationRequestedBy: identityTo);

        A.CallTo(() => fakeRelationshipsRepository.FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var handler = CreateHandler(fakeRelationshipsRepository, fakeUserContext);

        // Act
        var response = await handler.Handle(new RejectRelationshipReactivationCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        response.Id.Should().NotBeNull();
        response.Status.Should().Be(RelationshipStatus.Terminated);
        response.AuditLog.Should().HaveCount(5); // AuditLog(Creation->Acceptance->Termination->Reactivation->Rejection)
    }

    [Fact]
    public async Task Saves_the_updated_relationship()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var identityTo = TestDataGenerator.CreateRandomIdentityAddress();

        var mockRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreateRelationshipWithRequestedReactivation(from: activeIdentity, to: identityTo, reactivationRequestedBy: identityTo);
        A.CallTo(() => mockRelationshipsRepository.FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var handler = CreateHandler(mockRelationshipsRepository, fakeUserContext);

        // Act
        await handler.Handle(new RejectRelationshipReactivationCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        A.CallTo(
                () => mockRelationshipsRepository.Update(
                    A<Relationship>.That.Matches(r => r.Id == relationship.Id && r.Status == RelationshipStatus.Terminated))
            )
            .MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IRelationshipsRepository relationshipsRepository, IUserContext userContext)
    {
        return new Handler(relationshipsRepository, userContext);
    }
}
