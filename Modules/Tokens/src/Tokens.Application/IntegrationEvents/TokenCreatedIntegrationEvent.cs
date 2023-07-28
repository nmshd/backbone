﻿using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Tokens.Application.IntegrationEvents;
public class TokenCreatedIntegrationEvent : IntegrationEvent
{
    public TokenCreatedIntegrationEvent(Token newToken) : base($"{newToken.Id}/Created")
    {
        TokenId = newToken.Id;
        CreatedBy = newToken.CreatedBy;
    }

    public string TokenId { get; set; }
    public string CreatedBy { get; set; }
}
