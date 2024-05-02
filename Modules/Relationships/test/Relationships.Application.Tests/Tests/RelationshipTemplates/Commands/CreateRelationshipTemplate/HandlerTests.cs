using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.Entities;
using FakeItEasy;
using FluentAssertions.Execution;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class HandlerTests
{
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IEventBus _eventBus;

    public HandlerTests()
    {
        _userContext = A.Fake<IUserContext>();
        _mapper = A.Fake<IMapper>();
        _eventBus = A.Fake<IEventBus>();
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;
    }

    [Fact]
    public async Task Triggers_RelationshipTemplateCreatedDomainEvent()
    {
        // Arrange
        var command = new CreateRelationshipTemplateCommand
        {
            ExpiresAt = DateTime.UtcNow,
            Content = [1, 1, 1, 1, 1, 1, 1, 1]
        };

        var relationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();
        A.CallTo(() => relationshipTemplatesRepository.Add(A<RelationshipTemplate>._, CancellationToken.None)).Returns(Task.CompletedTask);
        A.CallTo(() => _userContext.GetAddress()).Returns("some-identity-address");
        A.CallTo(() => _userContext.GetDeviceId()).Returns(DeviceId.Parse("DVCsomedeviceid12345"));

        var handler = CreateHandler(relationshipTemplatesRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _eventBus.Publish(A<RelationshipTemplateCreatedDomainEvent>._)).MustHaveHappened();
    }

    private Handler CreateHandler(IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        return new Handler(relationshipTemplatesRepository, _userContext, _mapper, _eventBus);
    }
}
