using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
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
    public async Task Publishes_RelationshipCreatedIntegrationEvent()
    {
        // Arrange
        SystemTime.Set("2020-01-01");
        var activeIdentity = IdentityAddress.Create([0], "id1");

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

        var mockEventBus = A.Fake<IEventBus>();

        var handler = CreateHandler(fakeRelationshipTemplatesRepository, fakeUserContext, mockEventBus);

        // Act
        await handler.Handle(new CreateRelationshipCommand
        {
            RelationshipTemplateId = relationshipTemplate.Id
        }, CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.Publish(A<RelationshipCreatedIntegrationEvent>.That.Matches(e => e.From == activeIdentity, relationshipTemplate.CreatedBy))).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext? userContext = null, IEventBus? eventBus = null)
    {
        userContext ??= A.Dummy<IUserContext>();
        eventBus ??= A.Dummy<IEventBus>();
        var mapper = A.Dummy<IMapper>();
        var relationshipsRepository = A.Dummy<IRelationshipsRepository>();

        var handler = new Handler(userContext, mapper, eventBus, relationshipsRepository, relationshipTemplatesRepository);
        return handler;
    }
}
