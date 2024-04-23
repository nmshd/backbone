using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.CreateRelationship;

public class HandlerTests
{
    [Fact]
    public async Task Saves_the_created_relationship()
    {
        // Arrange
        SystemTime.Set("2020-01-01");
        var activeIdentity = IdentityAddress.Create([0], "id1");
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var fakeRelationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();
        var relationshipTemplate = new RelationshipTemplate(
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomDeviceId(),
            1,
            DateTime.Parse("2021-01-01"),
            []
        );
        A.CallTo(() => fakeRelationshipTemplatesRepository.Find(relationshipTemplate.Id, activeIdentity, A<CancellationToken>._, A<bool>._))
            .Returns(relationshipTemplate);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var mockRelationshipsRepository = A.Fake<IRelationshipsRepository>();

        var handler = CreateHandler(fakeRelationshipTemplatesRepository, fakeUserContext, mockRelationshipsRepository);

        // Act
        await handler.Handle(new CreateRelationshipCommand
        {
            RelationshipTemplateId = relationshipTemplate.Id
        }, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipsRepository.Add(A<Relationship>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Returns_data_of_created_relationship()
    {
        // Arrange
        SystemTime.Set("2020-01-01");
        var activeIdentity = IdentityAddress.Create([0], "id1");
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var fakeRelationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();
        var relationshipTemplate = new RelationshipTemplate(
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomDeviceId(),
            1,
            DateTime.Parse("2021-01-01"),
            []
        );
        A.CallTo(() => fakeRelationshipTemplatesRepository.Find(relationshipTemplate.Id, activeIdentity, A<CancellationToken>._, A<bool>._))
            .Returns(relationshipTemplate);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var handler = CreateHandler(fakeRelationshipTemplatesRepository, fakeUserContext);

        // Act
        var response = await handler.Handle(new CreateRelationshipCommand
        {
            RelationshipTemplateId = relationshipTemplate.Id
        }, CancellationToken.None);

        // Assert
        response.Id.Should().NotBeNull();
        response.RelationshipTemplateId.Should().Be(relationshipTemplate.Id);
        response.From.Should().Be(activeIdentity);
        response.To.Should().Be(relationshipTemplate.CreatedBy);
        response.CreatedAt.Should().Be(SystemTime.UtcNow);
        response.Status.Should().Be(RelationshipStatus.Pending);
        response.AuditLog.Should().HaveCount(1);
    }

    [Fact]
    public void Throws_when_no_template_with_given_id_exists()
    {
        // Arrange
        var relationshipTemplateId = RelationshipTemplateId.Parse("RLTNonExistingReltId");
        var address = IdentityAddress.Create([0], "id1");

        var relationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();
        A.CallTo(() => relationshipTemplatesRepository.Find(relationshipTemplateId, address, A<CancellationToken>._, A<bool>._))
            .Returns<RelationshipTemplate?>(null);

        var handler = CreateHandler(relationshipTemplatesRepository);

        // Act
        var acting = () => handler.Handle(new CreateRelationshipCommand
        {
            RelationshipTemplateId = relationshipTemplateId
        }, CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException, CreateRelationshipResponse>().Which.Message.Should().Contain("RelationshipTemplate");
    }

    [Fact]
    public async Task Publishes_RelationshipCreatedDomainEvent()
    {
        // Arrange
        SystemTime.Set("2020-01-01");
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var fakeRelationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();
        var relationshipTemplate = new RelationshipTemplate(
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomDeviceId(),
            1,
            DateTime.Parse("2021-01-01"),
            []
        );
        A.CallTo(() => fakeRelationshipTemplatesRepository.Find(relationshipTemplate.Id, activeIdentity, A<CancellationToken>._, A<bool>._))
            .Returns(relationshipTemplate);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var mockEventBus = A.Fake<IEventBus>();

        var handler = CreateHandler(fakeRelationshipTemplatesRepository, fakeUserContext, mockEventBus);

        // Act
        await handler.Handle(new CreateRelationshipCommand
        {
            RelationshipTemplateId = relationshipTemplate.Id
        }, CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.Publish(A<RelationshipStatusChangedDomainEvent>.That.Matches(e => e.Initiator == activeIdentity, relationshipTemplate.CreatedBy))).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        return CreateHandler(relationshipTemplatesRepository, null, null, null);
    }

    private static Handler CreateHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext, IEventBus eventBus)
    {
        return CreateHandler(relationshipTemplatesRepository, userContext, eventBus, null);
    }

    private static Handler CreateHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext)
    {
        return CreateHandler(relationshipTemplatesRepository, userContext, null, null);
    }

    private static Handler CreateHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        return CreateHandler(relationshipTemplatesRepository, userContext, null, relationshipsRepository);
    }

    private static Handler CreateHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext? userContext, IEventBus? eventBus,
        IRelationshipsRepository? relationshipsRepository)
    {
        userContext ??= A.Dummy<IUserContext>();
        eventBus ??= A.Dummy<IEventBus>();
        relationshipsRepository ??= A.Dummy<IRelationshipsRepository>();

        var handler = new Handler(userContext, eventBus, relationshipsRepository, relationshipTemplatesRepository);
        return handler;
    }
}
