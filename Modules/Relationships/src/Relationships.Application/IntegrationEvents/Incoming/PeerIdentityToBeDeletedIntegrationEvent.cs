﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Incoming;

public class PeerIdentityToBeDeletedIntegrationEvent : IntegrationEvent
{
    public string Address { get; set; }
    public string DeletionProcessId { get; set; }
}
