﻿using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipTemplateCreated;
public class RelationshipTemplateCreatedIntegrationEvent : IntegrationEvent
{
    public string CreatedBy { get; set; }
}
