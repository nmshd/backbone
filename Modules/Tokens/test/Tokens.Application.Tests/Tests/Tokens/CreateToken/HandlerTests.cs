﻿using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.IntegrationEvents;
using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using Backbone.Modules.Tokens.Domain.Entities;
using FakeItEasy;
using FluentAssertions.Execution;
using Xunit;

namespace Backbone.Modules.Tokens.Application.Tests.Tests.Tokens.CreateToken;
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
    public async void Triggers_TokenCreatedIntegrationEvent()
    {
        // Arrange
        var command = new CreateTokenCommand
        {
            ExpiresAt = DateTime.UtcNow,
            Content = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 }
        };

        var tokenRepository = A.Fake<ITokensRepository>();
        A.CallTo(() => tokenRepository.Add(A<Token>._)).Returns(Task.CompletedTask);
        A.CallTo(() => _userContext.GetAddress()).Returns("some-identity-address");
        A.CallTo(() => _userContext.GetDeviceId()).Returns(DeviceId.Parse("DVCsomedeviceid12345"));

        var handler = CreateHandler(tokenRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _eventBus.Publish(A<IntegrationEvent>.That.IsInstanceOf(typeof(TokenCreatedIntegrationEvent)))).MustHaveHappened();
    }

    private Handler CreateHandler(ITokensRepository tokens)
    {
        return new Handler(_userContext, _mapper, _eventBus, tokens);
    }
}
