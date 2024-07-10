using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationship;
using Backbone.Modules.Relationships.Application.Tests.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.AcceptRelationship;

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

        var fakeRelationshipTemplatesRepository = SetupFakeRelationshipTemplatesRepository(activeIdentity, activeDevice, relationship);

        var handler = CreateHandler(fakeUserContext, fakeRelationshipsRepository, fakeRelationshipTemplatesRepository);

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

        var fakeRelationshipTemplatesRepository = SetupFakeRelationshipTemplatesRepository(activeIdentity, activeDevice, relationship);

        var handler = CreateHandler(fakeUserContext, mockRelationshipsRepository, fakeRelationshipTemplatesRepository);

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

    private static Handler CreateHandler(IUserContext userContext, IRelationshipsRepository relationshipsRepository, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        return new Handler(userContext, relationshipsRepository, relationshipTemplatesRepository);
    }

    private static IRelationshipTemplatesRepository SetupFakeRelationshipTemplatesRepository(IdentityAddress activeIdentity, DeviceId activeDevice, Relationship relationship)
    {
        var fakeRelationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();
        var relationshipTemplate = new RelationshipTemplate(
            activeIdentity,
            activeDevice,
            1,
            DateTime.Parse("2021-01-01"),
            []);

        A.CallTo(() => fakeRelationshipTemplatesRepository.Find(relationship.RelationshipTemplateId, activeIdentity, A<CancellationToken>._, A<bool>._))
            .Returns(relationshipTemplate);

        return fakeRelationshipTemplatesRepository;
    }
}
